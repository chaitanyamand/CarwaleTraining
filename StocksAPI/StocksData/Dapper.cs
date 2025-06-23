using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;

namespace StocksAPI.Data
{
    /*
     * DapperContext is a central utility class for managing Dapper-based database connections.
     * It reads the connection string from configuration (appsettings.json) and provides a method
     * to instantiate MySQL connections as IDbConnection, enabling Dapper usage across the project.
     */
    public class DapperContext
    {
        private readonly IConfiguration _configuration;  // Access to configuration settings
        private readonly string _connectionString;       // Cached connection string for performance

        /*
         * Constructor receives injected IConfiguration, which includes access to appsettings.json.
         * It extracts the "DefaultConnection" string defined in configuration.
         */
        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        /*
         * CreateConnection creates and returns a new MySQL connection using the connection string.
         * Caller is responsible for managing the connection lifecycle (i.e., opening and disposing).
         *
         * Usage Example:
         * using var connection = _context.CreateConnection();
         * await connection.QueryAsync(...);
         */
        public IDbConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}
