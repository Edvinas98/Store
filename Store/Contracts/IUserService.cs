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
    public interface IUserService
    {
        Task<List<User>> GetAllUsers();
        Task<string> AddNewUser(User user);
        Task<Employee> GetEmployeeById(int id);
        Task<Customer> GetCustomerById(int id);
        Task<List<User>> GetUserByNameAndSurname(string name, string surname);
        Task<string> UpdateUser(User user);
        Task DeleteUser(User user);
        Task<List<Employee>> GetAllEmployees();
        Task<List<Customer>> GetAllCustomers();
        Task DeleteCache();
    }
}
