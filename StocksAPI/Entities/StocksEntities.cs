using StocksAPI.Enums;

namespace StocksAPI.Entities
{
    // Entity representing search filters for stocks
    public class Filters
    {
        public decimal? MinimumBudget { get; set; }
        public decimal? MaximumBudget { get; set; }
        public List<FuelType>? FuelTypes { get; set; }
        public string? MakeName { get; set; }
        public string? ModelName { get; set; }
        public int? MinYear { get; set; }
        public int? MaxYear { get; set; }
        public int? MaxKilometers { get; set; }
        public string? Location { get; set; }
    }

    // Entity representing stock data
    public class Stock
    {
        public int Id { get; set; }
        public string MakeName { get; set; } = string.Empty;
        public string ModelName { get; set; } = string.Empty;
        public int MakeYear { get; set; }
        public decimal Price { get; set; }
        public int Kilometers { get; set; }
        public FuelType FuelType { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}