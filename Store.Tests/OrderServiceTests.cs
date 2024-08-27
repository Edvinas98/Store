using Castle.Core.Resource;
using Moq;
using Serilog;
using Store.Core.Contracts;
using Store.Core.Enums;
using Store.Core.Models;
using Store.Core.Repositories;
using Store.Core.Services;
using static MongoDB.Driver.WriteConcern;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Store.Tests
{
    public class OrderServiceTests
    {
        [Fact]
        public async Task GetAllOrders()
        {
            //Arrange
            Mock<IOrderRepository> _orderDbRepository = new Mock<IOrderRepository>();
            Mock<IOrderCacheRepository> _orderCacheRepository = new Mock<IOrderCacheRepository>();
            Mock<IUserService> userService = new Mock<IUserService>();
            Mock<IProductService> productService = new Mock<IProductService>();
            Mock<ILogger> _logger = new Mock<ILogger>();

            Employee employee1 = new Employee
            {
                Id = 15,
                Name = "Petras",
                Surname = "Petraitis",
                Email = "petras@gmail.com",
                PhoneNumber = "37061234567"
            };

            Employee employee2 = new Employee
            {
                Id = 13,
                Name = "Jonas",
                Surname = "Jonaitis",
                Email = "jonas@gmail.com",
                PhoneNumber = "37067654321"
            };

            Employee employee3 = new Employee
            {
                Id = 9,
                Name = "Lukas",
                Surname = "Lukaitis",
                Email = "lukas@gmail.com",
                PhoneNumber = "37061111111"
            };

            Customer customer1 = new Customer
            {
                Id = 22,
                Name = "Rokas",
                Surname = "Rokaitis",
                Email = "rokas@gmail.com",
                PhoneNumber = "37061284567"
            };

            Customer customer2 = new Customer
            {
                Id = 3,
                Name = "Justas",
                Surname = "Justaitis",
                Email = "justas@gmail.com",
                PhoneNumber = "37067644321"
            };

            Customer customer3 = new Customer
            {
                Id = 9,
                Name = "Vardenis",
                Surname = "Pavardenis",
                Email = "vardenis@gmail.com",
                PhoneNumber = "37061112111"
            };

            Product product1 = new Product
            {
                Id = 14,
                Name = "Bread",
                Price = 2.59M,
                Stock = 4,
                Category = (ProductCategory)1
            };

            Product product2 = new Product
            {
                Id = 21,
                Name = "Toilet paper",
                Price = 3.19M,
                Stock = 7,
                Category = (ProductCategory)11
            };

            Product product3 = new Product
            {
                Id = 16,
                Name = "Shirt",
                Price = 9.99M,
                Stock = 6,
                Category = (ProductCategory)7
            };

            Order order1 = new Order
            {
                Id = 5,
                Employee = employee2,
                Customer = customer3,
                OrderProduct = product1,
                Date = DateOnly.FromDateTime(DateTime.Now),
                Amount = 3
            };

            Order order2 = new Order
            {
                Id = 9,
                Employee = employee3,
                Customer = customer1,
                OrderProduct = product2,
                Date = DateOnly.FromDateTime(DateTime.Now),
                Amount = 2
            };

            Order order3 = new Order
            {
                Id = 14,
                Employee = employee1,
                Customer = customer2,
                OrderProduct = product3,
                Date = DateOnly.FromDateTime(DateTime.Now),
                Amount = 4
            };

            List<Order> orders = new List<Order>();
            orders.Add(order1);
            orders.Add(order2);
            orders.Add(order3);

            _orderDbRepository.Setup(x => x.GetAllOrders()).Returns(Task.FromResult(orders));
            _orderCacheRepository.Setup(x => x.GetAllOrders()).Returns(Task.FromResult(orders));
            _orderCacheRepository.Setup(x => x.AddOrders(new List<Order>()));

            IOrderService orderService = new OrderService(userService.Object, productService.Object, _orderDbRepository.Object, _orderCacheRepository.Object, _logger.Object);

            //Act
            List<Order> items = await orderService.GetAllOrders();
            //Assert
            Assert.Equal(items, orders);
        }

        [Fact]
        public async Task AddNewOrder()
        {
            //Arrange
            Mock<IOrderRepository> _orderDbRepository = new Mock<IOrderRepository>();
            Mock<IOrderCacheRepository> _orderCacheRepository = new Mock<IOrderCacheRepository>();
            Mock<IUserService> userService = new Mock<IUserService>();
            Mock<IProductService> productService = new Mock<IProductService>();
            Mock<ILogger> _logger = new Mock<ILogger>();

            Employee employee = new Employee
            {
                Id = 15,
                Name = "Petras",
                Surname = "Petraitis",
                Email = "petras@gmail.com",
                PhoneNumber = "37061234567"
            };

            Customer customer = new Customer
            {
                Id = 22,
                Name = "Rokas",
                Surname = "Rokaitis",
                Email = "rokas@gmail.com",
                PhoneNumber = "37061284567"
            };

            Product product = new Product
            {
                Id = 14,
                Name = "Bread",
                Price = 2.59M,
                Stock = 4,
                Category = (ProductCategory)1
            };

            Order order = new Order
            {
                Id = 5,
                Employee = employee,
                Customer = customer,
                OrderProduct = product,
                Date = DateOnly.FromDateTime(DateTime.Now),
                Amount = 3
            };

            _orderDbRepository.Setup(x => x.AddNewOrder(order)).Returns(Task.FromResult(order));
            _orderCacheRepository.Setup(x => x.AddNewOrder(order));
            IOrderService orderService = new OrderService(userService.Object, productService.Object, _orderDbRepository.Object, _orderCacheRepository.Object, _logger.Object);

            //Act
            string result = await orderService.AddNewOrder(order);
            //Assert
            Assert.Equal(result, string.Empty);
        }

        [Fact]
        public async Task GetOrderById()
        {
            //Arrange
            Mock<IOrderRepository> _orderDbRepository = new Mock<IOrderRepository>();
            Mock<IOrderCacheRepository> _orderCacheRepository = new Mock<IOrderCacheRepository>();
            Mock<IUserService> userService = new Mock<IUserService>();
            Mock<IProductService> productService = new Mock<IProductService>();
            Mock<ILogger> _logger = new Mock<ILogger>();

            Employee employee1 = new Employee
            {
                Id = 15,
                Name = "Petras",
                Surname = "Petraitis",
                Email = "petras@gmail.com",
                PhoneNumber = "37061234567"
            };

            Employee employee2 = new Employee
            {
                Id = 13,
                Name = "Jonas",
                Surname = "Jonaitis",
                Email = "jonas@gmail.com",
                PhoneNumber = "37067654321"
            };

            Employee employee3 = new Employee
            {
                Id = 9,
                Name = "Lukas",
                Surname = "Lukaitis",
                Email = "lukas@gmail.com",
                PhoneNumber = "37061111111"
            };

            Customer customer1 = new Customer
            {
                Id = 22,
                Name = "Rokas",
                Surname = "Rokaitis",
                Email = "rokas@gmail.com",
                PhoneNumber = "37061284567"
            };

            Customer customer2 = new Customer
            {
                Id = 3,
                Name = "Justas",
                Surname = "Justaitis",
                Email = "justas@gmail.com",
                PhoneNumber = "37067644321"
            };

            Customer customer3 = new Customer
            {
                Id = 9,
                Name = "Vardenis",
                Surname = "Pavardenis",
                Email = "vardenis@gmail.com",
                PhoneNumber = "37061112111"
            };

            Product product1 = new Product
            {
                Id = 14,
                Name = "Bread",
                Price = 2.59M,
                Stock = 4,
                Category = (ProductCategory)1
            };

            Product product2 = new Product
            {
                Id = 21,
                Name = "Toilet paper",
                Price = 3.19M,
                Stock = 7,
                Category = (ProductCategory)11
            };

            Product product3 = new Product
            {
                Id = 16,
                Name = "Shirt",
                Price = 9.99M,
                Stock = 6,
                Category = (ProductCategory)7
            };

            Order order1 = new Order
            {
                Id = 5,
                Employee = employee2,
                Customer = customer3,
                OrderProduct = product1,
                Date = DateOnly.FromDateTime(DateTime.Now),
                Amount = 3
            };

            Order order2 = new Order
            {
                Id = 9,
                Employee = employee3,
                Customer = customer1,
                OrderProduct = product2,
                Date = DateOnly.FromDateTime(DateTime.Now),
                Amount = 2
            };

            Order order3 = new Order
            {
                Id = 14,
                Employee = employee1,
                Customer = customer2,
                OrderProduct = product3,
                Date = DateOnly.FromDateTime(DateTime.Now),
                Amount = 4
            };

            _orderDbRepository.Setup(x => x.GetOrderById(14)).Returns(Task.FromResult(order3));
            _orderCacheRepository.Setup(x => x.GetOrderById(14)).Returns(Task.FromResult(order3));
            _orderCacheRepository.Setup(x => x.AddNewOrder(new Order()));

            IOrderService orderService = new OrderService(userService.Object, productService.Object, _orderDbRepository.Object, _orderCacheRepository.Object, _logger.Object);

            //Act
            Order item = await orderService.GetOrderById(14);
            //Assert
            Assert.Equal(item, order3);
        }

        [Fact]
        public async Task GetOrdersByEmployee()
        {
            //Arrange
            Mock<IOrderRepository> _orderDbRepository = new Mock<IOrderRepository>();
            Mock<IOrderCacheRepository> _orderCacheRepository = new Mock<IOrderCacheRepository>();
            Mock<IUserService> userService = new Mock<IUserService>();
            Mock<IProductService> productService = new Mock<IProductService>();
            Mock<ILogger> _logger = new Mock<ILogger>();

            Employee employee1 = new Employee
            {
                Id = 15,
                Name = "Petras",
                Surname = "Petraitis",
                Email = "petras@gmail.com",
                PhoneNumber = "37061234567"
            };

            Employee employee2 = new Employee
            {
                Id = 13,
                Name = "Jonas",
                Surname = "Jonaitis",
                Email = "jonas@gmail.com",
                PhoneNumber = "37067654321"
            };

            Employee employee3 = new Employee
            {
                Id = 9,
                Name = "Lukas",
                Surname = "Lukaitis",
                Email = "lukas@gmail.com",
                PhoneNumber = "37061111111"
            };

            Customer customer1 = new Customer
            {
                Id = 22,
                Name = "Rokas",
                Surname = "Rokaitis",
                Email = "rokas@gmail.com",
                PhoneNumber = "37061284567"
            };

            Customer customer2 = new Customer
            {
                Id = 3,
                Name = "Justas",
                Surname = "Justaitis",
                Email = "justas@gmail.com",
                PhoneNumber = "37067644321"
            };

            Customer customer3 = new Customer
            {
                Id = 9,
                Name = "Vardenis",
                Surname = "Pavardenis",
                Email = "vardenis@gmail.com",
                PhoneNumber = "37061112111"
            };

            Product product1 = new Product
            {
                Id = 14,
                Name = "Bread",
                Price = 2.59M,
                Stock = 4,
                Category = (ProductCategory)1
            };

            Product product2 = new Product
            {
                Id = 21,
                Name = "Toilet paper",
                Price = 3.19M,
                Stock = 7,
                Category = (ProductCategory)11
            };

            Product product3 = new Product
            {
                Id = 16,
                Name = "Shirt",
                Price = 9.99M,
                Stock = 6,
                Category = (ProductCategory)7
            };

            Order order1 = new Order
            {
                Id = 5,
                Employee = employee2,
                Customer = customer3,
                OrderProduct = product1,
                Date = DateOnly.FromDateTime(DateTime.Now),
                Amount = 3
            };

            Order order2 = new Order
            {
                Id = 9,
                Employee = employee3,
                Customer = customer1,
                OrderProduct = product2,
                Date = DateOnly.FromDateTime(DateTime.Now),
                Amount = 2
            };

            Order order3 = new Order
            {
                Id = 14,
                Employee = employee1,
                Customer = customer2,
                OrderProduct = product3,
                Date = DateOnly.FromDateTime(DateTime.Now),
                Amount = 4
            };

            List<Order> orders = new List<Order>();
            orders.Add(order2);

            _orderDbRepository.Setup(x => x.GetOrdersByEmployee(employee3)).Returns(Task.FromResult(orders));
            _orderCacheRepository.Setup(x => x.GetOrdersByEmployee(employee3)).Returns(Task.FromResult(orders));
            IOrderService orderService = new OrderService(userService.Object, productService.Object, _orderDbRepository.Object, _orderCacheRepository.Object, _logger.Object);

            //Act
            List<Order> items = await orderService.GetOrdersByEmployee(employee3);
            //Assert
            Assert.Equal(items, orders);
        }

        [Fact]
        public async Task GetOrdersByCustomer()
        {
            //Arrange
            Mock<IOrderRepository> _orderDbRepository = new Mock<IOrderRepository>();
            Mock<IOrderCacheRepository> _orderCacheRepository = new Mock<IOrderCacheRepository>();
            Mock<IUserService> userService = new Mock<IUserService>();
            Mock<IProductService> productService = new Mock<IProductService>();
            Mock<ILogger> _logger = new Mock<ILogger>();

            Employee employee1 = new Employee
            {
                Id = 15,
                Name = "Petras",
                Surname = "Petraitis",
                Email = "petras@gmail.com",
                PhoneNumber = "37061234567"
            };

            Employee employee2 = new Employee
            {
                Id = 13,
                Name = "Jonas",
                Surname = "Jonaitis",
                Email = "jonas@gmail.com",
                PhoneNumber = "37067654321"
            };

            Employee employee3 = new Employee
            {
                Id = 9,
                Name = "Lukas",
                Surname = "Lukaitis",
                Email = "lukas@gmail.com",
                PhoneNumber = "37061111111"
            };

            Customer customer1 = new Customer
            {
                Id = 22,
                Name = "Rokas",
                Surname = "Rokaitis",
                Email = "rokas@gmail.com",
                PhoneNumber = "37061284567"
            };

            Customer customer2 = new Customer
            {
                Id = 3,
                Name = "Justas",
                Surname = "Justaitis",
                Email = "justas@gmail.com",
                PhoneNumber = "37067644321"
            };

            Customer customer3 = new Customer
            {
                Id = 9,
                Name = "Vardenis",
                Surname = "Pavardenis",
                Email = "vardenis@gmail.com",
                PhoneNumber = "37061112111"
            };

            Product product1 = new Product
            {
                Id = 14,
                Name = "Bread",
                Price = 2.59M,
                Stock = 4,
                Category = (ProductCategory)1
            };

            Product product2 = new Product
            {
                Id = 21,
                Name = "Toilet paper",
                Price = 3.19M,
                Stock = 7,
                Category = (ProductCategory)11
            };

            Product product3 = new Product
            {
                Id = 16,
                Name = "Shirt",
                Price = 9.99M,
                Stock = 6,
                Category = (ProductCategory)7
            };

            Order order1 = new Order
            {
                Id = 5,
                Employee = employee2,
                Customer = customer3,
                OrderProduct = product1,
                Date = DateOnly.FromDateTime(DateTime.Now),
                Amount = 3
            };

            Order order2 = new Order
            {
                Id = 9,
                Employee = employee3,
                Customer = customer1,
                OrderProduct = product2,
                Date = DateOnly.FromDateTime(DateTime.Now),
                Amount = 2
            };

            Order order3 = new Order
            {
                Id = 14,
                Employee = employee1,
                Customer = customer2,
                OrderProduct = product3,
                Date = DateOnly.FromDateTime(DateTime.Now),
                Amount = 4
            };

            List<Order> orders = new List<Order>();
            orders.Add(order2);

            _orderDbRepository.Setup(x => x.GetOrdersByCustomer(customer1)).Returns(Task.FromResult(orders));
            _orderCacheRepository.Setup(x => x.GetOrdersByCustomer(customer1)).Returns(Task.FromResult(orders));
            IOrderService orderService = new OrderService(userService.Object, productService.Object, _orderDbRepository.Object, _orderCacheRepository.Object, _logger.Object);

            //Act
            List<Order> items = await orderService.GetOrdersByCustomer(customer1);
            //Assert
            Assert.Equal(items, orders);
        }

        [Fact]
        public async Task GetOrdersByProduct()
        {
            //Arrange
            Mock<IOrderRepository> _orderDbRepository = new Mock<IOrderRepository>();
            Mock<IOrderCacheRepository> _orderCacheRepository = new Mock<IOrderCacheRepository>();
            Mock<IUserService> userService = new Mock<IUserService>();
            Mock<IProductService> productService = new Mock<IProductService>();
            Mock<ILogger> _logger = new Mock<ILogger>();

            Employee employee1 = new Employee
            {
                Id = 15,
                Name = "Petras",
                Surname = "Petraitis",
                Email = "petras@gmail.com",
                PhoneNumber = "37061234567"
            };

            Employee employee2 = new Employee
            {
                Id = 13,
                Name = "Jonas",
                Surname = "Jonaitis",
                Email = "jonas@gmail.com",
                PhoneNumber = "37067654321"
            };

            Employee employee3 = new Employee
            {
                Id = 9,
                Name = "Lukas",
                Surname = "Lukaitis",
                Email = "lukas@gmail.com",
                PhoneNumber = "37061111111"
            };

            Customer customer1 = new Customer
            {
                Id = 22,
                Name = "Rokas",
                Surname = "Rokaitis",
                Email = "rokas@gmail.com",
                PhoneNumber = "37061284567"
            };

            Customer customer2 = new Customer
            {
                Id = 3,
                Name = "Justas",
                Surname = "Justaitis",
                Email = "justas@gmail.com",
                PhoneNumber = "37067644321"
            };

            Customer customer3 = new Customer
            {
                Id = 9,
                Name = "Vardenis",
                Surname = "Pavardenis",
                Email = "vardenis@gmail.com",
                PhoneNumber = "37061112111"
            };

            Product product1 = new Product
            {
                Id = 14,
                Name = "Bread",
                Price = 2.59M,
                Stock = 4,
                Category = (ProductCategory)1
            };

            Product product2 = new Product
            {
                Id = 21,
                Name = "Toilet paper",
                Price = 3.19M,
                Stock = 7,
                Category = (ProductCategory)11
            };

            Product product3 = new Product
            {
                Id = 16,
                Name = "Shirt",
                Price = 9.99M,
                Stock = 6,
                Category = (ProductCategory)7
            };

            Order order1 = new Order
            {
                Id = 5,
                Employee = employee2,
                Customer = customer3,
                OrderProduct = product1,
                Date = DateOnly.FromDateTime(DateTime.Now),
                Amount = 3
            };

            Order order2 = new Order
            {
                Id = 9,
                Employee = employee3,
                Customer = customer1,
                OrderProduct = product2,
                Date = DateOnly.FromDateTime(DateTime.Now),
                Amount = 2
            };

            Order order3 = new Order
            {
                Id = 14,
                Employee = employee1,
                Customer = customer2,
                OrderProduct = product3,
                Date = DateOnly.FromDateTime(DateTime.Now),
                Amount = 4
            };

            List<Order> orders = new List<Order>();
            orders.Add(order2);

            _orderDbRepository.Setup(x => x.GetOrdersByProduct(product2)).Returns(Task.FromResult(orders));
            _orderCacheRepository.Setup(x => x.GetOrdersByProduct(product2)).Returns(Task.FromResult(orders));
            IOrderService orderService = new OrderService(userService.Object, productService.Object, _orderDbRepository.Object, _orderCacheRepository.Object, _logger.Object);

            //Act
            List<Order> items = await orderService.GetOrdersByProduct(product2);
            //Assert
            Assert.Equal(items, orders);
        }
    }
}