using Dapper;
using FinanceService.Entities;
using FinanceService.DAL.Interfaces;
using StocksAPI.Data;

namespace FinanceService.DAL.Repositories
{
    /*
     * Data Access Layer implementation for Stock entity using Dapper.
     * Handles database operations related to stock data.
     */
    public class StockDAL : IStockDAL
    {
        private readonly DapperContext _context;
        private readonly ILogger<StockDAL> _logger;

        // Constructor injecting Dapper context and logger
        public StockDAL(DapperContext context, ILogger<StockDAL> logger)
        {
            _context = context;
            _logger = logger;
        }

        /*
         * Retrieves active stock records for the given list of car IDs.
         * Filters out inactive records and logs the result.
         */
        public async Task<List<Stock>> GetStocksByIdsAsync(List<int> carIds)
        {
            try
            {
                using var connection = _context.CreateConnection();

                var sql = @"
                    SELECT 
                        Id, MakeName, ModelName, MakeYear, Price, Kilometers, 
                        FuelType, CityName, CreatedDate, IsActive, ImageUrl, 
                        StockImages, EmiText, TagText
                    FROM Stocks 
                    WHERE Id IN @CarIds AND IsActive = 1";

                var parameters = new { CarIds = carIds };

                var stocks = await connection.QueryAsync<Stock>(sql, parameters);

                _logger.LogInformation($"Retrieved {stocks.Count()} stocks from database");
                return stocks.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving stocks from database");
                throw new Exception($"Error retrieving stocks from database: {ex.Message}", ex);
            }
        }
    }
}
