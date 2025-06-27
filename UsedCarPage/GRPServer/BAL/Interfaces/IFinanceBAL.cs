using FinanceService.Entities;

namespace FinanceService.BAL.Interfaces
{
    /*
     * Interface for Finance Business Access Layer (BAL).
     * Defines contract for finance-related business operations.
     */
    public interface IFinanceBAL
    {
        /*
         * Determines which cars are considered value for money based on business logic.
         */
        Task<List<CarStatusEntity>> GetCarValueForMoneyStatusAsync(List<int> carIds);
    }
}
