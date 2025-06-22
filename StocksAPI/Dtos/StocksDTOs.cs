using System.ComponentModel.DataAnnotations;

namespace StocksAPI.DTOs
{
    // DTO for mapping query parameters of stock search request
    public class StockSearchRequestDTO
    {
        [Range(0, double.MaxValue, ErrorMessage = "Minimum budget must be greater than or equal to 0")]
        public decimal? MinBudget { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Maximum budget must be greater than or equal to 0")]
        public decimal? MaxBudget { get; set; }

        [StringLength(100, ErrorMessage = "Make name cannot exceed 100 characters")]
        public string? MakeName { get; set; }

        [StringLength(100, ErrorMessage = "Model name cannot exceed 100 characters")]
        public string? ModelName { get; set; }

        [Range(1900, 2030, ErrorMessage = "Minimum year must be between 1900 and 2030")]
        public int? MinYear { get; set; }

        [Range(1900, 2030, ErrorMessage = "Maximum year must be between 1900 and 2030")]
        public int? MaxYear { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Maximum kilometers must be greater than or equal to 0")]
        public int? MaxKilometers { get; set; }

        [StringLength(100, ErrorMessage = "Location cannot exceed 100 characters")]
        public string? Location { get; set; }

        public string? FuelTypes { get; set; } // Comma-separated fuel type values

        [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
        public int PageSize { get; set; } = 10;

        [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0")]
        public int PageNumber { get; set; } = 1;
    }

    // DTO for returning stock data with formatted fields
    public class StockDTO
    {
        public int Id { get; set; }
        public string MakeName { get; set; } = string.Empty;
        public string ModelName { get; set; } = string.Empty;
        public int MakeYear { get; set; }
        public decimal Price { get; set; }
        public string FormattedPrice { get; set; } = string.Empty;
        public string CarName { get; set; } = string.Empty;
        public int Kilometers { get; set; }
        public string FuelType { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public bool IsValueForMoney { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    // DTO for API response with pagination
    public class StockSearchResponseDTO
    {
        public List<StockDTO> Stocks { get; set; } = new List<StockDTO>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }
}