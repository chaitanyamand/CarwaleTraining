namespace StocksAPI.DTOs
{
    /* 
     * DTO for returning a single stock record with formatted and display-friendly fields.
     */
    public class StockDTO
    {
        public int ProfileId { get; set; }
        public string MakeName { get; set; } = string.Empty;
        public string ModelName { get; set; } = string.Empty;
        public int MakeYear { get; set; }
        public decimal Price { get; set; }
        public string FormattedPrice { get; set; } = string.Empty; /* Price formatted for display */
        public string CarName { get; set; } = string.Empty;
        public int Km { get; set; }
        public string Fuel { get; set; } = string.Empty;
        public string CityName { get; set; } = string.Empty;
        public bool IsValueForMoney { get; set; }
        public DateTime CreatedDate { get; set; }

        public List<string> StockImages { get; set; } = new List<string>(); 
        public string ImageUrl { get; set; } = string.Empty;
        public string EmiText { get; set; } = string.Empty; /* Formatted EMI text for display */
        public string TagText { get; set; } = string.Empty; /* Formatted Tag text for display */
    }
}