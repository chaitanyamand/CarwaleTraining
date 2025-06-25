using AutoMapper;
using StocksAPI.DTOs;
using StocksAPI.Entities;
using FinanceService.Protos;
using StocksAPI.BAL.Interfaces;
using StocksAPI.DAL.Interfaces;
using System.Web;

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
        private readonly IConfiguration _configuration; // Configuration for accessing app settings

        // Constructor with dependencies injected
        public StockBAL(IStockDAL stockDAL, IMapper mapper, Finance.FinanceClient grpcClient, IConfiguration configuration)
        {
            _stockDAL = stockDAL ?? throw new ArgumentNullException(nameof(stockDAL));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _grpcClient = grpcClient;
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
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
                var hasNextPage = request.PageNumber < totalPages;
                var hasPreviousPage = request.PageNumber > 1;

                Console.WriteLine($"Total Count: {totalCount}, Total Pages: {totalPages}, " +
                                  $"Has Next: {hasNextPage}, Has Previous: {hasPreviousPage}");

                // Generate navigation URLs
                var nextPageUrl = hasNextPage ? GeneratePageUrl(request, request.PageNumber + 1) : null;
                var previousPageUrl = hasPreviousPage ? GeneratePageUrl(request, request.PageNumber - 1) : null;

                return new StockSearchResponseDTO
                {
                    Stocks = stockDTOs,
                    TotalCount = totalCount,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalPages = totalPages,
                    HasNextPage = hasNextPage,
                    HasPreviousPage = hasPreviousPage,
                    NextPageUrl = nextPageUrl,
                    PreviousPageUrl = previousPageUrl
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

        /*
         * Helper method to generate paginated URL for next/previous page navigation.
         */
        private string? GeneratePageUrl(StockSearchRequestDTO request, int pageNumber)
        {
            var baseUrl = _configuration["ApiSettings:BaseUrl"];
            if (string.IsNullOrEmpty(baseUrl))
                return null;

            var endpoint = "/api/stocks/";
            var fullBaseUrl = $"{baseUrl.TrimEnd('/')}{endpoint}";

            var queryParams = new List<string>();

            // Add all non-null parameters to query string
            if (!string.IsNullOrEmpty(request.Budget))
                queryParams.Add($"Budget={HttpUtility.UrlEncode(request.Budget)}");

            if (!string.IsNullOrEmpty(request.MakeName))
                queryParams.Add($"MakeName={HttpUtility.UrlEncode(request.MakeName)}");

            if (!string.IsNullOrEmpty(request.ModelName))
                queryParams.Add($"ModelName={HttpUtility.UrlEncode(request.ModelName)}");

            if (request.MinYear.HasValue)
                queryParams.Add($"MinYear={request.MinYear.Value}");

            if (request.MaxYear.HasValue)
                queryParams.Add($"MaxYear={request.MaxYear.Value}");

            if (request.MaxKilometers.HasValue)
                queryParams.Add($"MaxKilometers={request.MaxKilometers.Value}");

            if (!string.IsNullOrEmpty(request.Location))
                queryParams.Add($"Location={HttpUtility.UrlEncode(request.Location)}");

            if (!string.IsNullOrEmpty(request.Fuel))
                queryParams.Add($"Fuel={HttpUtility.UrlEncode(request.Fuel)}");

            queryParams.Add($"PageSize={request.PageSize}");
            queryParams.Add($"PageNumber={pageNumber}");

            var queryString = string.Join("&", queryParams);
            return $"{fullBaseUrl}?{queryString}";
        }
    }
}