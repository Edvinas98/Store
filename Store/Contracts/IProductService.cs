using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Store.Core.Enums;
using Store.Core.Models;
using Store.Core.Repositories;

namespace Store.Core.Contracts
{
    public interface IProductService
    {
        Task<List<Product>> GetAllProducts();
        Task<string> AddNewProduct(Product product);
        Task<string> AddStockToProduct(Product product, int amount);
        Task<Product> GetProductById(int id);
        Task<Product> GetProductByName(string name);
        Task<string> UpdateProduct(Product product);
        Task DeleteProduct(Product product);
        Task DeleteCache();
    }
}
