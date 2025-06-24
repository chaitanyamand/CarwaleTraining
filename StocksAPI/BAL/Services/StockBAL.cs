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
        private readonly FinanceService.Protos.Finance.FinanceClient _grpcClient;  //GRPC Client 

        // Constructor with dependencies injected
        public StockBAL(IStockDAL stockDAL, IMapper mapper, FinanceService.Protos.Finance.FinanceClient grpcClient)
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


                var carIds = stocks.Select(s => s.Id).ToList();

                if (!carIds.Any())
                {
                    return new StockSearchResponseDTO
                    {
                        Stocks = new List<StockDTO>(),
                        TotalCount = 0,
                        PageNumber = request.PageNumber,
                        PageSize = request.PageSize,
                        TotalPages = 0,
                        HasNextPage = false,
                        HasPreviousPage = false
                    };
                }

                var grpcRequest = new ValueForMoneyRequest();
                grpcRequest.CarIds.AddRange(carIds);

                // Use RPC Server To Determine Business Logic Value
                var grpcResponse = await _grpcClient.GetIsValueForMoneyAsync(grpcRequest);

                // Map the result back to DTOs
                var valueMap = grpcResponse.CarStatuses.ToDictionary(c => c.Id, c => c.IsValueForMoney);

                foreach (var stockDTO in stockDTOs)
                {
                    if (valueMap.TryGetValue(stockDTO.ProfileId, out var isValue))
                    {
                        stockDTO.IsValueForMoney = isValue;
                    }
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
                
                // Prepare gRPC request to determine value for money    
                var grpcRequest = new ValueForMoneyRequest();
                grpcRequest.CarIds.Add(stock.Id);

                // Call gRPC service to get value for money status
                var grpcResponse = await _grpcClient.GetIsValueForMoneyAsync(grpcRequest);
                var carStatus = grpcResponse.CarStatuses.FirstOrDefault(c => c.Id == stock.Id);

                stockDTO.IsValueForMoney = carStatus?.IsValueForMoney ?? false; 

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
