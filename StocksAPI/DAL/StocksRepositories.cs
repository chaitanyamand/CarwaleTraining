using StocksAPI.Entities;
using StocksAPI.Enums;

namespace StocksAPI.DAL
{
    // Interface for Stock Data Access Layer
    public interface IStockDAL
    {
        Task<List<Stock>> GetStocksAsync(Filters filters, int pageNumber, int pageSize);
        Task<int> GetStocksCountAsync(Filters filters);
        Task<Stock?> GetStockByIdAsync(int id);
    }

    // Implementation of Stock Data Access Layer
    public class StockDAL : IStockDAL
    {
        // Mock Data, Will Be Replaced With Dapper Context 
        private readonly List<Stock> _stocks;

        public StockDAL()
        {
            // Sample data - In real application, this would come from database
            _stocks = GenerateSampleStocks();
        }

        // Get stocks based on filters with pagination
        public async Task<List<Stock>> GetStocksAsync(Filters filters, int pageNumber, int pageSize)
        {
            try
            {
                var filteredStocks = _stocks.Where(s => s.IsActive).ToList();

                // Apply filters
                filteredStocks = ApplyFilters(filteredStocks, filters);

                // Apply pagination
                var stocks = filteredStocks
                    .OrderByDescending(s => s.CreatedDate)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return await Task.FromResult(stocks);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error occurred while fetching stocks", ex);
            }
        }

        // Get total count of stocks based on filters
        public async Task<int> GetStocksCountAsync(Filters filters)
        {
            try
            {
                var filteredStocks = _stocks.Where(s => s.IsActive).ToList();
                filteredStocks = ApplyFilters(filteredStocks, filters);
                return await Task.FromResult(filteredStocks.Count);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error occurred while counting stocks", ex);
            }
        }

        // Get stock by ID
        public async Task<Stock?> GetStockByIdAsync(int id)
        {
            try
            {
                var stock = _stocks.FirstOrDefault(s => s.Id == id && s.IsActive);
                return await Task.FromResult(stock);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error occurred while fetching stock with ID {id}", ex);
            }
        }

        // Apply filters to the stock list
        private static List<Stock> ApplyFilters(List<Stock> stocks, Filters filters)
        {
            if (filters.MinimumBudget.HasValue)
                stocks = stocks.Where(s => s.Price >= filters.MinimumBudget.Value).ToList();

            if (filters.MaximumBudget.HasValue)
                stocks = stocks.Where(s => s.Price <= filters.MaximumBudget.Value).ToList();

            if (filters.FuelTypes?.Any() == true)
                stocks = stocks.Where(s => filters.FuelTypes.Contains(s.FuelType)).ToList();

            if (!string.IsNullOrWhiteSpace(filters.MakeName))
                stocks = stocks.Where(s => s.MakeName.ToLower().Contains(filters.MakeName.ToLower())).ToList();

            if (!string.IsNullOrWhiteSpace(filters.ModelName))
                stocks = stocks.Where(s => s.ModelName.ToLower().Contains(filters.ModelName.ToLower())).ToList();

            if (filters.MinYear.HasValue)
                stocks = stocks.Where(s => s.MakeYear >= filters.MinYear.Value).ToList();

            if (filters.MaxYear.HasValue)
                stocks = stocks.Where(s => s.MakeYear <= filters.MaxYear.Value).ToList();

            if (filters.MaxKilometers.HasValue)
                stocks = stocks.Where(s => s.Kilometers <= filters.MaxKilometers.Value).ToList();

            if (!string.IsNullOrWhiteSpace(filters.Location))
                stocks = stocks.Where(s => s.Location.ToLower().Contains(filters.Location.ToLower())).ToList();

            return stocks;
        }

        // Generate sample stock data (Need To Add Image Field)
        private static List<Stock> GenerateSampleStocks()
        {
            return new List<Stock>
            {
                new Stock { Id = 1, MakeName = "Maruti", ModelName = "Swift", MakeYear = 2020, Price = 650000, Kilometers = 15000, FuelType = FuelType.Petrol, Location = "Mumbai", Color = "Red", CreatedDate = DateTime.Now.AddDays(-10) },
                new Stock { Id = 2, MakeName = "Hyundai", ModelName = "i20", MakeYear = 2021, Price = 800000, Kilometers = 8000, FuelType = FuelType.Petrol, Location = "Delhi", Color = "White", CreatedDate = DateTime.Now.AddDays(-5) },
                new Stock { Id = 3, MakeName = "Honda", ModelName = "City", MakeYear = 2019, Price = 900000, Kilometers = 25000, FuelType = FuelType.Diesel, Location = "Bangalore", Color = "Silver", CreatedDate = DateTime.Now.AddDays(-15) },
                new Stock { Id = 4, MakeName = "Toyota", ModelName = "Innova", MakeYear = 2022, Price = 1800000, Kilometers = 5000, FuelType = FuelType.Diesel, Location = "Chennai", Color = "Black", CreatedDate = DateTime.Now.AddDays(-3) },
                new Stock { Id = 5, MakeName = "Tata", ModelName = "Nexon", MakeYear = 2021, Price = 950000, Kilometers = 12000, FuelType = FuelType.Electric, Location = "Pune", Color = "Blue", CreatedDate = DateTime.Now.AddDays(-7) },
                new Stock { Id = 6, MakeName = "Mahindra", ModelName = "Scorpio", MakeYear = 2020, Price = 1200000, Kilometers = 18000, FuelType = FuelType.Diesel, Location = "Hyderabad", Color = "White", CreatedDate = DateTime.Now.AddDays(-12) },
                new Stock { Id = 7, MakeName = "Maruti", ModelName = "Alto", MakeYear = 2022, Price = 450000, Kilometers = 3000, FuelType = FuelType.CNG, Location = "Mumbai", Color = "Silver", CreatedDate = DateTime.Now.AddDays(-2) },
                new Stock { Id = 8, MakeName = "Ford", ModelName = "EcoSport", MakeYear = 2019, Price = 750000, Kilometers = 22000, FuelType = FuelType.Petrol, Location = "Kolkata", Color = "Red", CreatedDate = DateTime.Now.AddDays(-20) },
                new Stock { Id = 9, MakeName = "Nissan", ModelName = "Magnite", MakeYear = 2023, Price = 700000, Kilometers = 1500, FuelType = FuelType.Petrol, Location = "Ahmedabad", Color = "Orange", CreatedDate = DateTime.Now.AddDays(-1) },
                new Stock { Id = 10, MakeName = "Kia", ModelName = "Seltos", MakeYear = 2021, Price = 1300000, Kilometers = 14000, FuelType = FuelType.Hybrid, Location = "Jaipur", Color = "Grey", CreatedDate = DateTime.Now.AddDays(-8) },
                new Stock { Id = 11, MakeName = "Maruti", ModelName = "Baleno", MakeYear = 2023, Price = 180000, Kilometers = 5000, FuelType = FuelType.Petrol, Location = "Mumbai", Color = "Blue", CreatedDate = DateTime.Now.AddDays(-4) },
                new Stock { Id = 12, MakeName = "Honda", ModelName = "Amaze", MakeYear = 2022, Price = 150000, Kilometers = 8000, FuelType = FuelType.Petrol, Location = "Delhi", Color = "White", CreatedDate = DateTime.Now.AddDays(-6) }
            };
        }
    }
}