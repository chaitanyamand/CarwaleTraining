using StocksAPI.DTOs;

namespace StocksAPI.BAL.Interfaces
{
    /*
     * Interface for Stock Business Access Layer (BAL).
     * Defines contract for stock-related business logic.
     */
    public interface IStockBAL
    {
        /* Performs search for stocks with filters and returns paginated result */
        Task<StockSearchResponseDTO> SearchStocksAsync(StockSearchRequestDTO request);

        /* Retrieves a specific stock by ID */
        Task<StockDTO?> GetStockByIdAsync(int id);
    }
}