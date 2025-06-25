namespace StocksAPI.DTOs
{
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

        public string NextPageUrl { get; set; } = string.Empty;
        public string PreviousPageUrl { get; set; } = string.Empty;

    }
}
