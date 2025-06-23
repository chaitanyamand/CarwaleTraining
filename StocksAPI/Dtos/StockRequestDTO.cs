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
}