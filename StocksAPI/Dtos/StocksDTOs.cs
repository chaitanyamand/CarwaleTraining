using System.ComponentModel.DataAnnotations;

namespace StocksAPI.DTOs
{
    /* 
     * DTO for mapping query parameters of stock search requests.
     * Used in controller as [FromQuery] binding model.
     */
    public class StockSearchRequestDTO
    {
        /* Budget range in the format 'min-max', validated with RegEx */
        [RegularExpression(@"^\d+\s*-\s*\d+$", ErrorMessage = "Budget must be in the format 'min-max', e.g., '100-500'")]
        public string? Budget { get; set; }

        /* Optional make name, max length 100 */
        [StringLength(100, ErrorMessage = "Make name cannot exceed 100 characters")]
        public string? MakeName { get; set; }

        /* Optional model name, max length 100 */
        [StringLength(100, ErrorMessage = "Model name cannot exceed 100 characters")]
        public string? ModelName { get; set; }

        /* Optional minimum manufacture year, must be within 1900–2030 */
        [Range(1900, 2030, ErrorMessage = "Minimum year must be between 1900 and 2030")]
        public int? MinYear { get; set; }

        /* Optional maximum manufacture year, must be within 1900–2030 */
        [Range(1900, 2030, ErrorMessage = "Maximum year must be between 1900 and 2030")]
        public int? MaxYear { get; set; }

        /* Optional maximum kilometers driven, must be >= 0 */
        [Range(0, int.MaxValue, ErrorMessage = "Maximum kilometers must be greater than or equal to 0")]
        public int? MaxKilometers { get; set; }

        /* Optional location string, max length 100 */
        [StringLength(100, ErrorMessage = "Location cannot exceed 100 characters")]
        public string? Location { get; set; }

        /* Optional fuel types, + separated (e.g., "1+2+3") */
        public string? Fuel { get; set; }

        /* Page size for pagination, default 10, valid range 1–100 */
        [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
        public int PageSize { get; set; } = 10;

        /* Page number for pagination, default 1 */
        [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0")]
        public int PageNumber { get; set; } = 1;
    }

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
        public string ImageUrl { get; set; } = string.Empty;
        public List<string> StockImages { get; set; } = new List<string>();
        public string EmiText { get; set; } = string.Empty; /* Formatted EMI text for display */
        public string TagText { get; set; } = string.Empty; /* Formatted Tag text for display */
    }

    /*
     * DTO for search response including stock list, pagination info,
     * and additional image resources.
     */
    public class StockSearchResponseDTO
    {
        public List<StockDTO> Stocks { get; set; } = new List<StockDTO>(); /* List of stock results */
        public int TotalCount { get; set; } /* Total number of matching records */
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public List<string> StockImages { get; set; } = new List<string>();
        public bool HasPreviousPage { get; set; }

    }
}
