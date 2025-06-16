using serverAPI.Models;
using System.Collections.Generic;

namespace serverAPI.Services
{
    public interface IProductService
    {
        IEnumerable<Product> GetAll();
        Product? GetById(int id);
        Product Add(Product product);
        void Update(int id, Product product);
        void Delete(int id);
    }
}
