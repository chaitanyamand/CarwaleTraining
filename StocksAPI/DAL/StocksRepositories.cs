using Dapper;
using StocksAPI.Entities;
using StocksAPI.Enums;
using StocksAPI.Data;
using System.Data;

namespace StocksAPI.DAL
{
    /* Interface that defines the contract for the Stock Data Access Layer */
    public interface IStockDAL
    {
        Task<List<Stock>> GetStocksAsync(Filters filters, int pageNumber, int pageSize);
        Task<int> GetStocksCountAsync(Filters filters);
        Task<Stock?> GetStockByIdAsync(int id);
    }

    /* Implementation of the Stock Data Access Layer using Dapper and MySQL */
    public class StockDAL : IStockDAL
    {
        private readonly DapperContext _context;

        /* Constructor that accepts a DapperContext for DB connection management */
        public StockDAL(DapperContext context)
        {
            _context = context;
        }

        /* 
         * Fetches a paginated list of stocks based on the provided filters.
         * Utilizes dynamic SQL generation for filtering.
         */
        public async Task<List<Stock>> GetStocksAsync(Filters filters, int pageNumber, int pageSize)
        {
            var query = @"SELECT * FROM stocks
                          WHERE IsActive = 1
                          /**filters**/
                          ORDER BY CreatedDate DESC
                          LIMIT @Offset, @PageSize";

            using var connection = _context.CreateConnection();

            var sqlParams = new DynamicParameters();

            /* Build dynamic filter SQL and parameters */
            var filtersSql = BuildFilters(filters, sqlParams);

            /* Pagination parameters */
            sqlParams.Add("Offset", (pageNumber - 1) * pageSize);
            sqlParams.Add("PageSize", pageSize);

            /* Replace filter placeholder with actual conditions */
            var finalQuery = query.Replace("/**filters**/", filtersSql);

            /* Execute query and return result list */
            var result = await connection.QueryAsync<Stock>(finalQuery, sqlParams);
            return result.ToList();
        }

        /*
         * Returns the total count of stocks that match the given filters.
         * Useful for paginated response metadata.
         */
        public async Task<int> GetStocksCountAsync(Filters filters)
        {
            var query = @"SELECT COUNT(*) FROM stocks
                          WHERE IsActive = 1
                          /**filters**/";

            using var connection = _context.CreateConnection();

            var sqlParams = new DynamicParameters();
            var filtersSql = BuildFilters(filters, sqlParams);

            var finalQuery = query.Replace("/**filters**/", filtersSql);

            return await connection.ExecuteScalarAsync<int>(finalQuery, sqlParams);
        }

        /*
         * Retrieves a specific stock record by its ID.
         * Returns null if no matching stock is found or if it's inactive.
         */
        public async Task<Stock?> GetStockByIdAsync(int id)
        {
            var query = @"SELECT * FROM stocks WHERE Id = @Id AND IsActive = 1";

            using var connection = _context.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<Stock>(query, new { Id = id });
        }

        /*
         * Dynamically builds SQL WHERE conditions based on filters provided by the user.
         * Parameters are added using Dapper to prevent SQL injection.
         */
        private static string BuildFilters(Filters filters, DynamicParameters parameters)
        {
            var conditions = new List<string>();

            /* Minimum budget (in lakhs, converted to actual price) */
            if (filters.MinimumBudget.HasValue)
            {
                conditions.Add("Price >= @MinBudget");
                parameters.Add("MinBudget", filters.MinimumBudget.Value * 100000);
            }

            /* Maximum budget */
            if (filters.MaximumBudget.HasValue)
            {
                conditions.Add("Price <= @MaxBudget");
                parameters.Add("MaxBudget", filters.MaximumBudget.Value * 100000);
            }

            /* Fuel type filter (e.g., Petrol, Diesel) */
            if (filters.FuelTypes != null && filters.FuelTypes.Any())
        {
                conditions.Add($"FuelType IN @FuelTypes");
                parameters.Add("FuelTypes", filters.FuelTypes.Select(ft => (int)ft).ToArray());
            }

            /* Car make filter (case-insensitive partial match) */
            if (!string.IsNullOrWhiteSpace(filters.MakeName))
            {
                conditions.Add("LOWER(MakeName) LIKE @MakeName");
                parameters.Add("MakeName", $"%{filters.MakeName.ToLower()}%");
            }

            /* Car model filter (case-insensitive partial match) */
            if (!string.IsNullOrWhiteSpace(filters.ModelName))
            {
                conditions.Add("LOWER(ModelName) LIKE @ModelName");
                parameters.Add("ModelName", $"%{filters.ModelName.ToLower()}%");
            }

            /* Minimum make year */
            if (filters.MinYear.HasValue)
            {
                conditions.Add("MakeYear >= @MinYear");
                parameters.Add("MinYear", filters.MinYear.Value);
            }

            /* Maximum make year */
            if (filters.MaxYear.HasValue)
            {
                conditions.Add("MakeYear <= @MaxYear");
                parameters.Add("MaxYear", filters.MaxYear.Value);
            }

            /* Maximum kilometers driven */
            if (filters.MaxKilometers.HasValue)
            {
                conditions.Add("Kilometers <= @MaxKilometers");
                parameters.Add("MaxKilometers", filters.MaxKilometers.Value);
            }

            /* Location filter (case-insensitive partial match) */
            if (!string.IsNullOrWhiteSpace(filters.Location))
            {
                conditions.Add("LOWER(Location) LIKE @Location");
                parameters.Add("Location", $"%{filters.Location.ToLower()}%");
            }

            /* Return combined conditions with AND if any filters exist */
            return conditions.Count > 0 ? "AND " + string.Join(" AND ", conditions) : "";
        }
    }
}
