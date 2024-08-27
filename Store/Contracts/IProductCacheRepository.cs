using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Store.Core.Models;

namespace Store.Core.Contracts
{
    public interface IProductCacheRepository
    {
        Task<List<Product>> GetAllProducts();
        Task<Product> AddNewProduct(Product product);
        Task AddProducts(List<Product> products);
        Task<Product> GetProductById(int id);
        Task<Product> GetProductByName(string name);
        Task<string> CheckProduct(Product product);
        Task UpdateProduct(Product product);
        Task DeleteProduct(Product product);
        Task DeleteCache();
        Task<int> GetProductCount();
    }
}
