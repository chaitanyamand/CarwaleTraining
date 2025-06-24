using AutoMapper;
using StocksAPI.DAL;
using StocksAPI.DTOs;
using StocksAPI.Entities;
using FinanceService.Protos;
using StocksAPI.BAL.Interfaces;
using StocksAPI.DAL.Interfaces;

namespace StocksAPI.BAL.Services
{
    /*
     * Implementation of IStockBAL.
     * Contains business logic for searching and retrieving stock data.
     */
    public class StockBAL : IStockBAL
    {
        private readonly IStockDAL _stockDAL;     // DAL layer for data operations
        private readonly IMapper _mapper;         // AutoMapper instance for DTO <-> Entity conversions
        private readonly Finance.FinanceClient _grpcClient;  // gRPC client for external business logic

        // Constructor with dependencies injected
        public StockBAL(IStockDAL stockDAL, IMapper mapper, Finance.FinanceClient grpcClient)
        {
            _stockDAL = stockDAL ?? throw new ArgumentNullException(nameof(stockDAL));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _grpcClient = grpcClient;
        }

        /*
         * Searches for stocks based on filters in request.
         * Returns paginated stock list along with metadata.
         */
        public async Task<StockSearchResponseDTO> SearchStocksAsync(StockSearchRequestDTO request)
        {
            try
            {
                var filters = _mapper.Map<Filters>(request);

                // Parallelize DB operations
                var stocksTask = _stockDAL.GetStocksAsync(filters, request.PageNumber, request.PageSize);
                var countTask = _stockDAL.GetStocksCountAsync(filters);

                await Task.WhenAll(stocksTask, countTask);

                var stocks = await stocksTask;
                var totalCount = await countTask;
                var stockDTOs = _mapper.Map<List<StockDTO>>(stocks);

                var carIds = stocks.Select(s => s.Id).ToList();

                if (carIds.Any())
                {
                    // Fetch value-for-money statuses using gRPC
                    var valueMap = await GetValueForMoneyMapAsync(carIds);

                    foreach (var stockDTO in stockDTOs)
                    {
                        if (valueMap.TryGetValue(stockDTO.ProfileId, out var isValue))
                        {
                            stockDTO.IsValueForMoney = isValue;
                        }
                    }
                }

                var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

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
                var stock = await _stockDAL.GetStockByIdAsync(id);
                if (stock == null)
                    return null;

                var stockDTO = _mapper.Map<StockDTO>(stock);

                // Fetch value-for-money status using gRPC
                var valueMap = await GetValueForMoneyMapAsync(new List<int> { stock.Id });
                stockDTO.IsValueForMoney = valueMap.TryGetValue(stock.Id, out var isValue) && isValue;

                return stockDTO;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error occurred while fetching stock with ID {id}", ex);
            }
        }

        /*
         * Helper method to call gRPC service with a list of car IDs
         * and retrieve a dictionary mapping each ID to its value-for-money status.
         */
        private async Task<Dictionary<int, bool>> GetValueForMoneyMapAsync(List<int> carIds)
        {
            var grpcRequest = new ValueForMoneyRequest();
            grpcRequest.CarIds.AddRange(carIds);

            var grpcResponse = await _grpcClient.GetIsValueForMoneyAsync(grpcRequest);

            return grpcResponse.CarStatuses
                .ToDictionary(c => c.Id, c => c.IsValueForMoney);
        }
    }
}
