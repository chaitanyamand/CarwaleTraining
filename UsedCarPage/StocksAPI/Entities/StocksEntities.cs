using StocksAPI.Enums;

namespace StocksAPI.Entities
{
    /*
     * Entity representing a stock item (i.e., a car listing).
     * This is the core database model.
     */
    public class Stock
    {
        /* Unique stock ID */
        public int Id { get; set; }

        /* Manufacturer name */
        public string MakeName { get; set; } = string.Empty;

        /* Car model name */
        public string ModelName { get; set; } = string.Empty;

        /* Manufacturing year */
        public int MakeYear { get; set; }

        /* Price in rupees */
        public decimal Price { get; set; }

        /* Distance driven in kilometers */
        public int Kilometers { get; set; }

        /* Fuel type (enum) */
        public FuelType FuelType { get; set; }

        /* Location (city, region) */
        public string CityName { get; set; } = string.Empty;


        /* Record creation date (for sorting or audit) */
        public DateTime CreatedDate { get; set; }

        /* Indicates if the listing is active and visible */
        public bool IsActive { get; set; } = true;

        /* Primary image URL for the listing */
        public string ImageUrl { get; set; } = string.Empty;

        /* Optional additional images (e.g., interior, rear view) */
        public List<string> StockImages { get; set; } = new List<string>();
        /* Formatted EMI text for display */
        public string EmiText { get; set; } = string.Empty;

        public string TagText { get; set; } = string.Empty;
    }
}
