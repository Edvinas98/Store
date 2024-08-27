using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Store.Core.Contracts;
using Store.Core.Models;

namespace Store.Core.Repositories
{
    public class ProductDbRepository : IProductRepository
    {
        public async Task<int> GetProductCount()
        {
            using (var context = new MyDbContext())
            {
                return (await context.Products.ToListAsync()).Count();
            }
        }

        public async Task<Product> AddNewProduct(Product product)
        {
            using (var context = new MyDbContext())
            {
                await context.Products.AddAsync(product);
                await context.SaveChangesAsync();
            }
            return product;
        }

        public async Task<string> CheckProduct(Product product)
        {
            using (var context = new MyDbContext())
            {
                List<Product> nameProducts = await context.Products.Where<Product>(x => x.Id != product.Id && x.Name.ToLower() == product.Name.ToLower()).ToListAsync();
                if (nameProducts.Count > 0)
                    return "A product with such name is already registered";
            }
            return string.Empty;
        }

        public async Task DeleteProduct(Product product)
        {
            using (var context = new MyDbContext())
            {
                context.Products.Remove(product);
                await context.SaveChangesAsync();
            }
        }

        public async Task<List<Product>> GetAllProducts()
        {
            List<Product> products = new List<Product>();
            using (var context = new MyDbContext())
            {
                products = await context.Products.ToListAsync();
            }
            return products;
        }

        public async Task<Product> GetProductById(int id)
        {
            using (var context = new MyDbContext())
            {
                List<Product> products = await context.Products.Where(x => x.Id == id).ToListAsync();
                if (products.Count() > 0)
                    return products[0];
            }
            return new Product();
        }

        public async Task<Product> GetProductByName(string name)
        {
            using (var context = new MyDbContext())
            {
                List<Product> products = await context.Products.Where(x => x.Name.ToLower() == name.ToLower()).ToListAsync();
                if (products.Count() > 0)
                    return products[0];
            }
            return new Product();
        }

        public async Task UpdateProduct(Product product)
        {
            using (var context = new MyDbContext())
            {
                context.Products.Update(product);
                await context.SaveChangesAsync();
            }
        }
    }
}
