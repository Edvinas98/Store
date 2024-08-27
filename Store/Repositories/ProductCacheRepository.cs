using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using Store.Core.Contracts;
using Store.Core.Models;

namespace Store.Core.Repositories
{
    public class ProductCacheRepository : IProductCacheRepository
    {
        private readonly IMongoCollection<Product> _productsCache;

        public ProductCacheRepository(IMongoClient mongoClient)
        {
            _productsCache = mongoClient.GetDatabase("store").GetCollection<Product>("products_cache");
        }

        public Task DeleteCache()
        {
            _productsCache.Database.DropCollectionAsync("products_cache");
            return Task.CompletedTask;
        }

        public async Task<int> GetProductCount()
        {
            return (await _productsCache.FindAsync<Product>(x => x.Id != 0)).ToList().Count();
        }

        public async Task<Product> AddNewProduct(Product product)
        {
            await _productsCache.InsertOneAsync(product);
            return product;
        }

        public async Task AddProducts(List<Product> products)
        {
            if (products.Count > 0)
            {
                await _productsCache.Database.DropCollectionAsync("products_cache");
                await _productsCache.InsertManyAsync(products);
            }
        }

        public async Task AddProductToOrder(Order order)
        {
            order.OrderProduct = (await _productsCache.FindAsync<Product>(x => x.Id == order.OrderProduct.Id)).First();
        }

        public async Task AddProductToOrders(List<Order> orders)
        {
            List<Task> tasks = new List<Task>();
            foreach (Order order in orders)
            {
                tasks.Add(AddProductToOrder(order));
            }
            await Task.WhenAll(tasks);
        }

        public async Task<string> CheckProduct(Product product)
        {
            List<Product> nameProducts = (await _productsCache.FindAsync<Product>(x => x.Id != product.Id && x.Name.ToLower() == product.Name.ToLower())).ToList();
            if (nameProducts.Count > 0)
                return "A product with such name is already registered";
            return string.Empty;
        }

        public async Task DeleteProduct(Product product)
        {
            await _productsCache.DeleteOneAsync<Product>(x => x.Id == product.Id);
        }

        public async Task<List<Product>> GetAllProducts()
        {
            return (await _productsCache.FindAsync<Product>(x => x.Id != 0)).ToList();
        }

        public async Task<Product> GetProductById(int id)
        {
            List<Product> products = (await _productsCache.FindAsync<Product>(x => x.Id == id)).ToList();
            if (products.Count() > 0)
                return products[0];
            return new Product();
        }

        public async Task<Product> GetProductByName(string name)
        {
            List<Product> products = (await _productsCache.FindAsync<Product>(x => x.Name.ToLower() == name.ToLower())).ToList();
            if (products.Count() > 0)
                return products[0];
            return new Product();
        }

        public async Task UpdateProduct(Product product)
        {
            await _productsCache.ReplaceOneAsync<Product>(x => x.Id == product.Id, product);
        }
    }
}
