using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Store.Core.Contracts;
using Store.Core.Enums;
using Store.Core.Models;
using Store.Core.Repositories;

namespace Store.Core.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductCacheRepository _productCacheRepository;
        private readonly ILogger _logger;

        public ProductService(IProductRepository productRepository, IProductCacheRepository productCacheRepository, ILogger logger)
        {
            _productRepository = productRepository;
            _productCacheRepository = productCacheRepository;
            _logger = logger;
            _logger.Information("Product service started");
        }

        public async Task DeleteCache()
        {
            while (true)
            {
                await Task.Delay(2 * 60000);
                var deleteCache = _productCacheRepository.DeleteCache();
            }
        }

        private async Task<bool> VerifyCache()
        {
            var databaseCount = _productRepository.GetProductCount();
            var cacheCount = _productCacheRepository.GetProductCount();
            await Task.WhenAll(databaseCount, cacheCount);
            if (databaseCount.Result == cacheCount.Result)
                return true;
            return false;
        }

        public async Task<List<Product>> GetAllProducts()
        {
            List<Product> result = new List<Product>();
            _logger.Information("'GetAllProducts' was called");

            if (await VerifyCache())
            {
                _logger.Information("Loading products from cache");
                result = await _productCacheRepository.GetAllProducts();
            }
            else
            {
                _logger.Information("Loading orders from SQL");
                result = await _productRepository.GetAllProducts();
                await _productCacheRepository.AddProducts(result);
            }
            _logger.Information($"Loaded {result.Count()} products");
            return result;
        }

        public async Task<string> AddNewProduct(Product product)
        {
            _logger.Information($"'AddNewProduct' was called, product id: {product.Id}");
            string checkResult = await CheckProductData(product);
            if (!string.IsNullOrEmpty(checkResult))
            {
                _logger.Warning($"product adding aborted - {checkResult}");
                return checkResult;
            }

            if (await VerifyCache())
                checkResult = await _productCacheRepository.CheckProduct(product);
            else
                checkResult = await _productRepository.CheckProduct(product);

            if (string.IsNullOrEmpty(checkResult))
            {
                _logger.Information("Product was added successfully");
                await _productCacheRepository.AddNewProduct(await _productRepository.AddNewProduct(product));
            }
            else
                _logger.Warning($"product adding aborted - {checkResult}");
            return checkResult;
        }

        public async Task<string> AddStockToProduct(Product product, int amount)
        {
            _logger.Information($"'AddStockToProduct' was called, product id: {product.Id}, amount: {amount}");
            if (amount <= 0)
            {
                _logger.Warning($"product stock adding aborted because of bad amount format");
                return "Wrong amount format detected";
            }

            product.Stock += amount;
            return await UpdateProduct(product);
        }

        public async Task<Product> GetProductById(int id)
        {
            _logger.Information($"'GetProductById' was called, Id: {id}");
            Product product = await _productCacheRepository.GetProductById(id);
            if (product.Id != 0)
                return product;
            product = await _productRepository.GetProductById(id);
            if (product.Id != 0)
                await _productCacheRepository.AddNewProduct(product);
            _logger.Information($"Returning product with Id: {product.Id}");
            return product;
        }

        public async Task<Product> GetProductByName(string name)
        {
            _logger.Information($"'GetProductByName' was called, name: {name}");
            Product product = await _productCacheRepository.GetProductByName(name);
            if (product.Id != 0)
                return product;
            product = await _productRepository.GetProductByName(name);
            if (product.Id != 0)
                await _productCacheRepository.AddNewProduct(product);
            _logger.Information($"Returning product with Id: {product.Id}");
            return product;
        }

        public async Task<string> UpdateProduct(Product product)
        {
            _logger.Information($"'UpdateProduct' was called, product id: {product.Id}");
            string checkResult = await CheckProductData(product);
            if (!string.IsNullOrEmpty(checkResult))
            {
                _logger.Warning($"Aborting, reason: {checkResult}");
                return checkResult;
            }

            if (await VerifyCache())
                checkResult = await _productCacheRepository.CheckProduct(product);
            else
                checkResult = await _productRepository.CheckProduct(product);

            if (string.IsNullOrEmpty(checkResult))
            {
                _logger.Information("Product update was successful");
                var updateDb = _productRepository.UpdateProduct(product);
                var updateCache = _productCacheRepository.UpdateProduct(product);
                await Task.WhenAll(updateDb, updateCache);
            }
            else
                _logger.Warning($"Aborting, reason: {checkResult}");
            return checkResult;
        }

        public async Task DeleteProduct(Product product)
        {
            _logger.Information($"'DeleteProduct' was called, product id: {product.Id}");
            var updateDb = _productRepository.DeleteProduct(product);
            var updateCache = _productCacheRepository.DeleteProduct(product);
            await Task.WhenAll(updateDb, updateCache);
        }

        private Task<string> CheckProductData(Product product)
        {
            if (string.IsNullOrEmpty(product.Name))
                return Task.FromResult("Wrong name format detected");
            if (product.Price <= 0)
                return Task.FromResult("Wrong price format detected");
            if (product.Stock < 0)
                return Task.FromResult("Wrong stock format detected");
            if (product.Category == (ProductCategory)0)
                return Task.FromResult("Wrong product category detected");
            return Task.FromResult(string.Empty);
        }
    }
}
