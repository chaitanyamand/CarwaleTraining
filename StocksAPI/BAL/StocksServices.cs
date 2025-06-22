using AutoMapper;
using StocksAPI.DAL;
using StocksAPI.DTOs;
using StocksAPI.Entities;

namespace StocksAPI.BAL
{
    // Interface for Stock Business Access Layer
    public interface IStockBAL
    {
        Task<StockSearchResponseDTO> SearchStocksAsync(StockSearchRequestDTO request);
        Task<StockDTO?> GetStockByIdAsync(int id);
    }

    // Implementation of Stock Business Access Layer
    public class StockBAL : IStockBAL
    {
        private readonly IStockDAL _stockDAL;
        private readonly IMapper _mapper;

        public StockBAL(IStockDAL stockDAL, IMapper mapper)
        {
            _stockDAL = stockDAL ?? throw new ArgumentNullException(nameof(stockDAL));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        // Search stocks based on filters and return paginated response
        public async Task<StockSearchResponseDTO> SearchStocksAsync(StockSearchRequestDTO request)
        {
            try
            {
                // Map DTO to Filters entity
                var filters = _mapper.Map<Filters>(request);

                // Get stocks and total count
                var stocksTask = _stockDAL.GetStocksAsync(filters, request.PageNumber, request.PageSize);
                var countTask = _stockDAL.GetStocksCountAsync(filters);

                await Task.WhenAll(stocksTask, countTask);

                var stocks = await stocksTask;
                var totalCount = await countTask;

                // Map stocks to DTOs and set IsValueForMoney
                var stockDTOs = _mapper.Map<List<StockDTO>>(stocks);
                foreach (var stockDTO in stockDTOs)
                {
                    stockDTO.IsValueForMoney = DetermineValueForMoney(stocks.First(s => s.Id == stockDTO.Id));
                }

                // Calculate pagination info
                var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

                return new StockSearchResponseDTO
                {
                    Stocks = stockDTOs,
                    TotalCount = totalCount,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalPages = totalPages,
                    HasNextPage = request.PageNumber < totalPages,
                    HasPreviousPage = request.PageNumber > 1
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error occurred while searching stocks", ex);
            }
        }

        // Get stock by ID and return DTO
        public async Task<StockDTO?> GetStockByIdAsync(int id)
        {
            try
            {
                var stock = await _stockDAL.GetStockByIdAsync(id);
                if (stock == null)
                    return null;

                var stockDTO = _mapper.Map<StockDTO>(stock);
                stockDTO.IsValueForMoney = DetermineValueForMoney(stock);

                return stockDTO;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error occurred while fetching stock with ID {id}", ex);
            }
        }

        // Determine if a stock is value for money based on business rules
        private static bool DetermineValueForMoney(Stock stock)
        {
            // Business rule: kms < 10000 and price < 2L (200000) (Will Be Definitely Changed)
            return stock.Kilometers < 10000 && stock.Price < 200000;
        }
    }
}