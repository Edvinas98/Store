using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Store.Core.Models;
using Store.Core.Repositories;

namespace Store.Core.Contracts
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllUsers();
        Task<string> CheckUser(User user);
        Task<User> AddNewUser(User user);
        Task<Employee> GetEmployeeById(int id);
        Task<Customer> GetCustomerById(int id);
        Task<List<User>> GetUsersByNameAndSurname(string name, string surname);
        Task UpdateUser(User user);
        Task DeleteUser(User user);
        Task<List<Employee>> GetAllEmployees();
        Task<List<Customer>> GetAllCustomers();
        Task<int> GetEmployeeCount();
        Task<int> GetCustomerCount();
    }
}
