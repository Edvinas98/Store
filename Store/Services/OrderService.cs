using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Store.Core.Contracts;
using Store.Core.Enums;
using Store.Core.Models;
using Store.Core.Repositories;
using Serilog.Extensions.Logging;
using static MongoDB.Driver.WriteConcern;
using Serilog;

namespace Store.Core.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderCacheRepository _orderCacheRepository;
        private readonly ILogger _logger;

        public OrderService(IUserService userService, IProductService productService, IOrderRepository orderRepository, IOrderCacheRepository orderCacheRepository, ILogger logger)
        {
            _userService = userService;
            _productService = productService;
            _orderRepository = orderRepository;
            _orderCacheRepository = orderCacheRepository;
            _logger = logger;
            _logger.Information("Order service started");
        }

        ///////////////////////////////////////////////
        ///////////////// Orders //////////////////////
        ///////////////////////////////////////////////

        public async Task DeleteCache()
        {
            while (true)
            {
                await Task.Delay(2 * 60000);
                var deleteCache = _orderCacheRepository.DeleteCache();
            }
        }

        private async Task<bool> VerifyCache()
        {
            var databaseCount = _orderRepository.GetOrderCount();
            var cacheCount = _orderCacheRepository.GetOrderCount();
            await Task.WhenAll(databaseCount, cacheCount);
            if (databaseCount.Result == cacheCount.Result)
                return true;
            return false;
        }

        public async Task<List<Order>> GetAllOrders()
        {
            List<Order> result = new List<Order>();
            _logger.Information("'GetAllOrders' was called");

            if (await VerifyCache())
            {
                _logger.Information("Loading orders from cache");
                result = await _orderCacheRepository.GetAllOrders();
            }
            else
            {
                _logger.Information("Loading orders from SQL");
                result = await _orderRepository.GetAllOrders();
                await _orderCacheRepository.AddOrders(result);
            }
            _logger.Information($"Loaded {result.Count()} orders");
            return result;
        }

        public async Task<string> AddNewOrder(Order order)
        {
            _logger.Information($"'AddNewOrder' was called, order id: {order.Id}");
            if (order.Amount <= 0)
            {
                _logger.Warning($"Order adding aborted - Amount: {order.Amount}");
                return "Wrong amount format detected";
            }

            if (order.OrderProduct.Stock < order.Amount)
            {
                _logger.Warning($"Order adding aborted - product stock: {order.OrderProduct.Stock}, order amount: {order.Amount}");
                return "Specified amount is exceeding current stock of this product";
            }

            order.OrderProduct.Stock -= order.Amount;
            await UpdateProduct(order.OrderProduct);
            await _orderCacheRepository.AddNewOrder(await _orderRepository.AddNewOrder(order));
            _logger.Information("Order was successfully added");
            return string.Empty;
        }

        public async Task<Order> GetOrderById(int id)
        {
            _logger.Information($"'GetOrderById' was called, Id: {id}");
            Order order = await _orderCacheRepository.GetOrderById(id);
            if (order.Id != 0)
            {
                return order;
            }
            order = await _orderRepository.GetOrderById(id);
            if (order.Id != 0)
                await _orderCacheRepository.AddNewOrder(order);
            _logger.Information($"Returning order with Id: {order.Id}");
            return order;
        }

        public async Task<List<Order>> GetOrdersByEmployee(User user)
        {
            _logger.Information($"'GetOrdersByEmployee' was called, user id: {user.Id}");
            if (await VerifyCache())
            {
                return await _orderCacheRepository.GetOrdersByEmployee(user);
            }
            else
            {
                return await _orderRepository.GetOrdersByEmployee(user);
            }
        }

        public async Task<List<Order>> GetOrdersByCustomer(User user)
        {
            _logger.Information($"'GetOrdersByCustomer' was called, user Id: {user.Id}");
            if (await VerifyCache())
            {
                return await _orderCacheRepository.GetOrdersByCustomer(user);
            }
            else
            {
                return await _orderRepository.GetOrdersByCustomer(user);
            }
        }

        public async Task<List<Order>> GetOrdersByProduct(Product product)
        {
            _logger.Information($"'GetOrdersByProduct' was called, product Id: {product.Id}");
            if (await VerifyCache())
            {
                return await _orderCacheRepository.GetOrdersByProduct(product);

            }
            else
            {
                return await _orderRepository.GetOrdersByProduct(product);
            }
        }

        public Task<decimal> GetOrderPrice(Order order)
        {
            return Task.FromResult(order.OrderProduct.Price * order.Amount);
        }

        public async Task<string> UpdateOrder(Order order)
        {
            _logger.Information($"'UpdateOrder' was called, order id: {order.Id}");
            if (order.Amount <= 0)
            {
                _logger.Warning($"Aborting, order amount: {order.Amount}");
                return "Wrong amount format detected";
            }

            Order oldOrder = await GetOrderById(order.Id);
            if (order.Amount != oldOrder.Amount)
            {
                if (order.OrderProduct.Id != oldOrder.OrderProduct.Id)
                {
                    if (order.OrderProduct.Stock < order.Amount)
                    {
                        _logger.Warning($"Aborting, order amount: {order.Amount}, product stock: {order.OrderProduct.Stock}");
                        return "Specified amount is exceeding current stock of this product";
                    }
                    oldOrder.OrderProduct.Stock += oldOrder.Amount;
                    await UpdateProduct(oldOrder.OrderProduct);

                }
                else
                {
                    if (order.OrderProduct.Stock + oldOrder.Amount < order.Amount)
                    {
                        _logger.Warning($"Aborting, order amount: {order.Amount}, product stock: {order.OrderProduct.Stock + oldOrder.Amount}");
                        return "Specified amount is exceeding current stock of this product";
                    }
                    order.OrderProduct.Stock += oldOrder.Amount;
                }
                order.OrderProduct.Stock -= order.Amount;
                await UpdateProduct(order.OrderProduct);
                var updateDb = _orderRepository.UpdateOrder(order);
                var updateCache = _orderCacheRepository.UpdateOrder(order);
                await Task.WhenAll(updateDb, updateCache);
            }
            else
            {
                var updateDb = _orderRepository.UpdateOrder(order);
                var updateCache = _orderCacheRepository.UpdateOrder(order);
                await Task.WhenAll(updateDb, updateCache);
            }
            _logger.Information("Update was successful");
            return string.Empty;
        }

        public async Task DeleteOrder(Order order)
        {
            _logger.Information($"'DeleteOrder' was called, order id: {order.Id}");
            order.OrderProduct.Stock += order.Amount;
            var updateProduct = UpdateProduct(order.OrderProduct);
            var updateDb = _orderRepository.DeleteOrder(order);
            var updateCache = _orderCacheRepository.DeleteOrder(order);
            await Task.WhenAll(updateProduct, updateDb, updateCache);
        }

        private async Task DeleteOrdersByProduct(Product product)
        {
            _logger.Information($"'DeleteOrdersByProduct' was called, product id: {product.Id}");
            List<Task> tasks = new List<Task>();
            List<Order> orders = await GetOrdersByProduct(product);
            foreach (Order order in orders)
            {
                tasks.Add(_orderRepository.DeleteOrder(order));
                tasks.Add(_orderCacheRepository.DeleteOrder(order));
            }
            await Task.WhenAll(tasks);
        }

        private async Task DeleteOrdersByEmployee(User user)
        {
            _logger.Information($"'DeleteOrdersByEmployee' was called, user id: {user.Id}");
            List<Task> tasks = new List<Task>();
            List<Order> orders = await GetOrdersByEmployee(user);
            List<Product> products = new List<Product>();
            foreach (Order order in orders)
            {
                int i = products.FindIndex(x => x.Id == order.OrderProduct.Id);
                if (i >= 0)
                    products[i].Stock += order.Amount;
                else
                {
                    order.OrderProduct.Stock += order.Amount;
                    products.Add(order.OrderProduct);
                }
                tasks.Add(_orderRepository.DeleteOrder(order));
                tasks.Add(_orderCacheRepository.DeleteOrder(order));
            }
            foreach (Product product in products)
            {
                tasks.Add(UpdateProduct(product));
            }
            await Task.WhenAll(tasks);
        }

        private async Task DeleteOrdersByCustomer(User user)
        {
            _logger.Information($"'DeleteOrdersByCustomer' was called, user id: {user.Id}");
            List<Task> tasks = new List<Task>();
            List<Order> orders = await GetOrdersByCustomer(user);
            List<Product> products = new List<Product>();
            foreach (Order order in orders)
            {
                int i = products.FindIndex(x => x.Id == order.OrderProduct.Id);
                if (i >= 0)
                    products[i].Stock += order.Amount;
                else
                {
                    order.OrderProduct.Stock += order.Amount;
                    products.Add(order.OrderProduct);
                }
                tasks.Add(_orderRepository.DeleteOrder(order));
                tasks.Add(_orderCacheRepository.DeleteOrder(order));
            }
            foreach (Product product in products)
            {
                tasks.Add(UpdateProduct(product));
            }
            await Task.WhenAll(tasks);
        }

        ///////////////////////////////////////////////
        ///////////////// Products ////////////////////
        ///////////////////////////////////////////////

        public async Task<List<Product>> GetAllProducts()
        {
            return await _productService.GetAllProducts();
        }

        public async Task<string> AddNewProduct(Product product)
        {
            return await _productService.AddNewProduct(product);
        }

        public async Task<string> AddStockToProduct(Product product, int amount)
        {
            return await _productService.AddStockToProduct(product, amount);
        }

        public async Task<Product> GetProductById(int id)
        {
            return await _productService.GetProductById(id);
        }

        public async Task<Product> GetProductByName(string name)
        {
            return await _productService.GetProductByName(name);
        }

        public async Task<string> UpdateProduct(Product product)
        {
            return await _productService.UpdateProduct(product);
        }

        public async Task DeleteProduct(Product product)
        {
            await DeleteOrdersByProduct(product);
            await _productService.DeleteProduct(product);
        }

        ///////////////////////////////////////////////
        ///////////////// Users ///////////////////////
        ///////////////////////////////////////////////

        public async Task<List<User>> GetAllUsers()
        {
            return await _userService.GetAllUsers();
        }

        public async Task<string> AddNewUser(User user)
        {
            return await _userService.AddNewUser(user);
        }

        public async Task<Employee> GetEmployeeById(int id)
        {
            return await _userService.GetEmployeeById(id);
        }

        public async Task<Customer> GetCustomerById(int id)
        {
            return await _userService.GetCustomerById(id);
        }

        public async Task<List<User>> GetUserByNameAndSurname(string name, string surname)
        {
            return await _userService.GetUserByNameAndSurname(name, surname);
        }

        public async Task<string> UpdateUser(User user)
        {
            return await _userService.UpdateUser(user);
        }

        public async Task DeleteUser(User user)
        {
            if (user is Employee)
                await DeleteOrdersByEmployee((Employee)user);
            else
                await DeleteOrdersByCustomer((Customer)user);
            await _userService.DeleteUser(user);
        }

        public async Task<List<Employee>> GetAllEmployees()
        {
            return await _userService.GetAllEmployees();
        }
        public async Task<List<Customer>> GetAllCustomers()
        {
            return await _userService.GetAllCustomers();
        }
    }
}
