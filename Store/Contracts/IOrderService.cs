using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Store.Core.Models;
using Store.Core.Repositories;
using Store.Core.Services;

namespace Store.Core.Contracts
{
    public interface IOrderService
    {
        // Orders
        Task<List<Order>> GetAllOrders();
        Task<string> AddNewOrder(Order order);
        Task<Order> GetOrderById(int id);
        Task<List<Order>> GetOrdersByEmployee(User user);
        Task<List<Order>> GetOrdersByCustomer(User user);
        Task<List<Order>> GetOrdersByProduct(Product product);
        Task<decimal> GetOrderPrice(Order order);
        Task<string> UpdateOrder(Order order);
        Task DeleteOrder(Order order);
        Task DeleteCache();

        // Products
        Task<List<Product>> GetAllProducts();
        Task<string> AddNewProduct(Product product);
        Task<string> AddStockToProduct(Product product, int amount);
        Task<Product> GetProductById(int id);
        Task<Product> GetProductByName(string name);
        Task<string> UpdateProduct(Product product);
        Task DeleteProduct(Product product);

        // Users
        Task<List<User>> GetAllUsers();
        Task<string> AddNewUser(User user);
        Task<Employee> GetEmployeeById(int id);
        Task<Customer> GetCustomerById(int id);
        Task<List<User>> GetUserByNameAndSurname(string name, string surname);
        Task<string> UpdateUser(User user);
        Task DeleteUser(User user);
        Task<List<Employee>> GetAllEmployees();
        Task<List<Customer>> GetAllCustomers();
    }
}
