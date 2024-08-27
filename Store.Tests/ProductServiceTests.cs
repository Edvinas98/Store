using System.Diagnostics;
using Moq;
using Serilog;
using Store.Core.Contracts;
using Store.Core.Enums;
using Store.Core.Models;
using Store.Core.Services;
using static System.Reflection.Metadata.BlobBuilder;

namespace Store.Tests
{
    public class ProductServiceTests
    {
        [Fact]
        public async Task GetAllProducts()
        {
            //Arrange
            Mock<IProductRepository> _productDbRepository = new Mock<IProductRepository>();
            Mock<IProductCacheRepository> _productCacheRepository = new Mock<IProductCacheRepository>();
            Mock<ILogger> _logger = new Mock<ILogger>();

            Product product1 = new Product
            {
                Id = 15,
                Name = "Bread",
                Price = 2.59M,
                Stock = 4,
                Category = (ProductCategory)1
            };

            Product product2 = new Product
            {
                Id = 13,
                Name = "Toilet paper",
                Price = 3.19M,
                Stock = 7,
                Category = (ProductCategory)11
            };

            Product product3 = new Product
            {
                Id = 9,
                Name = "Shirt",
                Price = 9.99M,
                Stock = 6,
                Category = (ProductCategory)7
            };

            List<Product> products = new List<Product>();
            products.Add(product1);
            products.Add(product2);
            products.Add(product3);

            _productDbRepository.Setup(x => x.GetAllProducts()).Returns(Task.FromResult(products));
            _productCacheRepository.Setup(x => x.GetAllProducts()).Returns(Task.FromResult(products));
            _productCacheRepository.Setup(x => x.AddProducts(new List<Product>()));
            IProductService productService = new ProductService(_productDbRepository.Object, _productCacheRepository.Object, _logger.Object);

            //Act
            List<Product> items = await productService.GetAllProducts();
            //Assert
            Assert.Equal(items, products);
        }

        [Fact]
        public async Task AddNewProduct()
        {
            //Arrange
            Mock<IProductRepository> _productDbRepository = new Mock<IProductRepository>();
            Mock<IProductCacheRepository> _productCacheRepository = new Mock<IProductCacheRepository>();
            Mock<ILogger> _logger = new Mock<ILogger>();

            Product product = new Product
            {
                Id = 15,
                Name = "Bread",
                Price = 2.59M,
                Stock = 4,
                Category = (ProductCategory)1
            };

            _productDbRepository.Setup(x => x.CheckProduct(product)).Returns(Task.FromResult(string.Empty));
            _productCacheRepository.Setup(x => x.CheckProduct(product)).Returns(Task.FromResult(string.Empty));
            _productDbRepository.Setup(x => x.AddNewProduct(product)).Returns(Task.FromResult(product));
            _productCacheRepository.Setup(x => x.AddNewProduct(product)).Returns(Task.FromResult(product));
            IProductService productService = new ProductService(_productDbRepository.Object, _productCacheRepository.Object, _logger.Object);

            //Act
            string result = await productService.AddNewProduct(product);
            //Assert
            Assert.Equal(result, string.Empty);
        }

        [Fact]
        public async Task AddStockToProduct()
        {
            //Arrange
            Mock<IProductRepository> _productDbRepository = new Mock<IProductRepository>();
            Mock<IProductCacheRepository> _productCacheRepository = new Mock<IProductCacheRepository>();
            Mock<ILogger> _logger = new Mock<ILogger>();

            Product product = new Product
            {
                Id = 15,
                Name = "Bread",
                Price = 2.59M,
                Stock = 4,
                Category = (ProductCategory)1
            };

            IProductService productService = new ProductService(_productDbRepository.Object, _productCacheRepository.Object, _logger.Object);

            //Act
            await productService.AddStockToProduct(product, 5);
            int result = 9;
            //Assert
            Assert.Equal(product.Stock, result);
        }

        [Fact]
        public async Task GetProductById()
        {
            //Arrange
            Mock<IProductRepository> _productDbRepository = new Mock<IProductRepository>();
            Mock<IProductCacheRepository> _productCacheRepository = new Mock<IProductCacheRepository>();
            Mock<ILogger> _logger = new Mock<ILogger>();

            Product product = new Product
            {
                Id = 15,
                Name = "Bread",
                Price = 2.59M,
                Stock = 4,
                Category = (ProductCategory)1
            };

            _productDbRepository.Setup(x => x.GetProductById(15)).Returns(Task.FromResult(product));
            _productCacheRepository.Setup(x => x.GetProductById(15)).Returns(Task.FromResult(product));
            _productCacheRepository.Setup(x => x.AddNewProduct(product)).Returns(Task.FromResult(product));
            IProductService productService = new ProductService(_productDbRepository.Object, _productCacheRepository.Object, _logger.Object);

            //Act
            Product item = await productService.GetProductById(15);
            //Assert
            Assert.Equal(item, product);
        }

        [Fact]
        public async Task GetProductByName()
        {
            //Arrange
            Mock<IProductRepository> _productDbRepository = new Mock<IProductRepository>();
            Mock<IProductCacheRepository> _productCacheRepository = new Mock<IProductCacheRepository>();
            Mock<ILogger> _logger = new Mock<ILogger>();

            Product product = new Product
            {
                Id = 15,
                Name = "Bread",
                Price = 2.59M,
                Stock = 4,
                Category = (ProductCategory)1
            };

            _productDbRepository.Setup(x => x.GetProductByName("Bread")).Returns(Task.FromResult(product));
            _productCacheRepository.Setup(x => x.GetProductByName("Bread")).Returns(Task.FromResult(product));
            _productCacheRepository.Setup(x => x.AddNewProduct(product)).Returns(Task.FromResult(product));
            IProductService productService = new ProductService(_productDbRepository.Object, _productCacheRepository.Object, _logger.Object);

            //Act
            Product item = await productService.GetProductByName("Bread");
            //Assert
            Assert.Equal(item, product);
        }

        [Fact]
        public async Task UpdateProduct()
        {
            //Arrange
            Mock<IProductRepository> _productDbRepository = new Mock<IProductRepository>();
            Mock<IProductCacheRepository> _productCacheRepository = new Mock<IProductCacheRepository>();
            Mock<ILogger> _logger = new Mock<ILogger>();

            Product product = new Product
            {
                Id = 15,
                Name = "Bread",
                Price = 2.59M,
                Stock = 4,
                Category = (ProductCategory)1
            };

            _productDbRepository.Setup(x => x.CheckProduct(product)).Returns(Task.FromResult(string.Empty));
            _productCacheRepository.Setup(x => x.CheckProduct(product)).Returns(Task.FromResult(string.Empty));
            _productDbRepository.Setup(x => x.UpdateProduct(product));
            _productCacheRepository.Setup(x => x.UpdateProduct(product));
            IProductService productService = new ProductService(_productDbRepository.Object, _productCacheRepository.Object, _logger.Object);

            //Act
            string result = await productService.UpdateProduct(product);
            //Assert
            Assert.Equal(string.Empty, result);
        }
    }
}