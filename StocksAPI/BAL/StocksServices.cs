using AutoMapper;
using StocksAPI.DAL;
using StocksAPI.DTOs;
using StocksAPI.Entities;

namespace StocksAPI.BAL
{
    /*
     * Interface for Stock Business Access Layer (BAL).
     * Defines contract for stock-related business logic.
     */
    public interface IStockBAL
    {
        /* Performs search for stocks with filters and returns paginated result */
        Task<StockSearchResponseDTO> SearchStocksAsync(StockSearchRequestDTO request);

        /* Retrieves a specific stock by ID */
        Task<StockDTO?> GetStockByIdAsync(int id);
    }

    /*
     * Implementation of IStockBAL.
     * Contains business logic for searching and retrieving stock data.
     */
    public class StockBAL : IStockBAL
    {
        private readonly IStockDAL _stockDAL;     // DAL layer for data operations
        private readonly IMapper _mapper;         // AutoMapper instance for DTO <-> Entity conversions

        // Constructor with dependencies injected
        public StockBAL(IStockDAL stockDAL, IMapper mapper)
        {
            _stockDAL = stockDAL ?? throw new ArgumentNullException(nameof(stockDAL));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /*
         * Searches for stocks based on filters in request.
         * Returns paginated stock list along with metadata.
         */
        public async Task<StockSearchResponseDTO> SearchStocksAsync(StockSearchRequestDTO request)
        {
            try
            {
                // Map input DTO to internal Filters entity
                var filters = _mapper.Map<Filters>(request);

                // Run queries in parallel: stocks list and total count
                var stocksTask = _stockDAL.GetStocksAsync(filters, request.PageNumber, request.PageSize);
                var countTask = _stockDAL.GetStocksCountAsync(filters);

                await Task.WhenAll(stocksTask, countTask);

                var stocks = await stocksTask;
                var totalCount = await countTask;

                // Map entity list to DTOs
                var stockDTOs = _mapper.Map<List<StockDTO>>(stocks);

                // Apply business rule for each stock
                foreach (var stockDTO in stockDTOs)
                {
                    // Determine if it's value for money using stock entity (not DTO)
                    stockDTO.IsValueForMoney = DetermineValueForMoney(
                        stocks.First(s => s.Id == stockDTO.ProfileId)
                    );
                }

                // Calculate total number of pages
                var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

                // Return fully constructed response DTO
                return new StockSearchResponseDTO
                {
                    Stocks = stockDTOs,
                    TotalCount = totalCount,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalPages = totalPages,
                    HasNextPage = request.PageNumber < totalPages,
                    HasPreviousPage = request.PageNumber > 1,
                };
            }
            catch (Exception ex)
            {
                // Wrap and rethrow as business-layer-specific exception
                throw new InvalidOperationException("Error occurred while searching stocks", ex);
            }
        }

        /*
         * Fetches stock data by ID and returns as DTO.
         */
        public async Task<StockDTO?> GetStockByIdAsync(int id)
        {
            try
            {
                // Fetch from DB
                var stock = await _stockDAL.GetStockByIdAsync(id);
                if (stock == null)
                    return null;

                // Map entity to DTO
                var stockDTO = _mapper.Map<StockDTO>(stock);

                // Apply business rule
                stockDTO.IsValueForMoney = DetermineValueForMoney(stock);

                return stockDTO;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error occurred while fetching stock with ID {id}", ex);
            }
        }

        /*
         * Business rule to determine "value for money":
         * Less than 10,000 km driven AND price under 2 lakh.
         * NOTE: This is a placeholder and can be adjusted.
         */
        private static bool DetermineValueForMoney(Stock stock)
        {
            return stock.Kilometers < 10000 && stock.Price < 200000;
        }
    }
}
