using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Store.Core.Models;

namespace Store.Core.Contracts
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllProducts();
        Task<Product> AddNewProduct(Product product);
        Task<Product> GetProductById(int id);
        Task<Product> GetProductByName(string name);
        Task<string> CheckProduct(Product product);
        Task UpdateProduct(Product product);
        Task DeleteProduct(Product product);
        Task<int> GetProductCount();
    }
}
