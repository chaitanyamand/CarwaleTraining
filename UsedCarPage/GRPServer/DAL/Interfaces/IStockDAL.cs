using FinanceService.Entities;

namespace FinanceService.DAL.Interfaces
{
    /*
     * Interface for Data Access Layer related to Stock entities.
     * Defines contract for accessing stock data from the data source.
     */
    public interface IStockDAL
    {
        /*
         * Retrieves a list of stock records corresponding to the provided car IDs.
         */
        Task<List<Stock>> GetStocksByIdsAsync(List<int> carIds);
    }
}
