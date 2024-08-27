using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Store.Core.Models;

namespace Store.Core.Contracts
{
    public interface IUserCacheRepository
    {
        Task<List<User>> GetAllUsers();
        Task<string> CheckUser(User user);
        Task AddNewUser(User user);
        Task AddUsers(List<User> users);
        Task<Employee> GetEmployeeById(int id);
        Task<Customer> GetCustomerById(int id);
        Task<List<User>> GetUsersByNameAndSurname(string name, string surname);
        Task UpdateUser(User user);
        Task DeleteUser(User user);
        Task<List<Employee>> GetAllEmployees();
        Task<List<Customer>> GetAllCustomers();
        Task DeleteCache();
        Task<int> GetEmployeeCount();
        Task<int> GetCustomerCount();
    }
}
