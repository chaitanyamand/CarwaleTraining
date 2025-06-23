using StocksAPI.Entities;

namespace StocksAPI.DAL.Interfaces
{
    /* Interface that defines the contract for the Stock Data Access Layer */
    public interface IStockDAL
    {
        Task<List<Stock>> GetStocksAsync(Filters filters, int pageNumber, int pageSize);
        Task<int> GetStocksCountAsync(Filters filters);
        Task<Stock?> GetStockByIdAsync(int id);
    }
}