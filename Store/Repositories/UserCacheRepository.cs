using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Store.Core.Contracts;
using Store.Core.Models;

namespace Store.Core.Repositories
{
    public class UserCacheRepository : IUserCacheRepository
    {
        private readonly IMongoCollection<Employee> _employeesCache;
        private readonly IMongoCollection<Customer> _customersCache;

        public UserCacheRepository(IMongoClient mongoClient)
        {
            _employeesCache = mongoClient.GetDatabase("store").GetCollection<Employee>("employees_cache");
            _customersCache = mongoClient.GetDatabase("store").GetCollection<Customer>("customers_cache");
        }

        public Task DeleteCache()
        {
            _employeesCache.Database.DropCollectionAsync("employees_cache");
            _customersCache.Database.DropCollectionAsync("customers_cache");
            return Task.CompletedTask;
        }

        public async Task<int> GetEmployeeCount()
        {
            return (await _employeesCache.FindAsync<Employee>(x => x.Id != 0)).ToList().Count();
        }

        public async Task<int> GetCustomerCount()
        {
            return (await _customersCache.FindAsync<Customer>(x => x.Id != 0)).ToList().Count();
        }

        public async Task AddNewUser(User user)
        {
            if (user is Employee)
                await _employeesCache.InsertOneAsync((Employee)user);
            else
                await _customersCache.InsertOneAsync((Customer)user);
        }

        public async Task AddUsers(List<User> users)
        {
            List<Employee> employees = new List<Employee>();
            List<Customer> customers = new List<Customer>();
            foreach (User user in users)
            {
                if (user is Employee)
                    employees.Add((Employee)user);
                else
                    customers.Add((Customer)user);
            }
            var addEmployees = AddEmployees(employees);
            var addCustomers = AddCustomers(customers);
            await Task.WhenAll(addEmployees, addCustomers);
        }

        private async Task AddEmployees(List<Employee> employees)
        {
            if (employees.Count > 0)
            {
                await _employeesCache.Database.DropCollectionAsync("employees_cache");
                await _employeesCache.InsertManyAsync(employees);
            }
        }

        private async Task AddCustomers(List<Customer> customers)
        {
            if (customers.Count > 0)
            {
                await _customersCache.Database.DropCollectionAsync("customers_cache");
                await _customersCache.InsertManyAsync(customers);
            }
        }

        public async Task<string> CheckUser(User user)
        {
            if (user is Employee)
            {
                var emailEmployees = _employeesCache.FindAsync<Employee>(x => x.Id != ((Employee)user).Id && x.Email == user.Email);
                var phoneEmployees = _employeesCache.FindAsync<Employee>(x => x.Id != ((Employee)user).Id && x.PhoneNumber == user.PhoneNumber);
                await Task.WhenAll(emailEmployees, phoneEmployees);
                if (emailEmployees.Result.ToList().Count > 0)
                    return "A user with such email is already registered";
                if (phoneEmployees.Result.ToList().Count > 0)
                    return "A user with such phone number is already registered";
            }
            else
            {
                var emailCustomers = _customersCache.FindAsync<Customer>(x => x.Id != ((Customer)user).Id && x.Email == user.Email);
                var phoneCustomers = _customersCache.FindAsync<Customer>(x => x.Id != ((Customer)user).Id && x.PhoneNumber == user.PhoneNumber);
                await Task.WhenAll(emailCustomers, phoneCustomers);

                if (emailCustomers.Result.ToList().Count > 0)
                    return "A user with such email is already registered";
                if (phoneCustomers.Result.ToList().Count > 0)
                    return "A user with such phone number is already registered";
            }
            return string.Empty;
        }

        public async Task<List<User>> GetAllUsers()
        {
            var employees = GetAllEmployees();
            var customers = GetAllCustomers();
            await Task.WhenAll(employees, customers);
            List<User> users = new List<User>();
            users.AddRange(employees.Result);
            users.AddRange(customers.Result);
            return users;
        }

        public async Task<List<Employee>> GetAllEmployees()
        {
            return (await _employeesCache.FindAsync<Employee>(x => x.Id != 0)).ToList();
        }

        public async Task<List<Customer>> GetAllCustomers()
        {
            return (await _customersCache.FindAsync<Customer>(x => x.Id != 0)).ToList();
        }

        public async Task<Employee> GetEmployeeById(int id)
        {
            List<Employee> users = (await _employeesCache.FindAsync<Employee>(x => x.Id == id)).ToList();
            if (users.Count() > 0)
                return users[0];
            return new Employee();
        }

        public async Task<Customer> GetCustomerById(int id)
        {
            List<Customer> users = (await _customersCache.FindAsync<Customer>(x => x.Id == id)).ToList();
            if (users.Count() > 0)
                return users[0];
            return new Customer();
        }

        public async Task<List<User>> GetUsersByNameAndSurname(string name, string surname)
        {
            var employees = GetEmployeesByNameAndSurname(name, surname);
            var customers = GetCustomersByNameAndSurname(name, surname);
            await Task.WhenAll(employees, customers);
            List<User> users = new List<User>();
            users.AddRange(employees.Result);
            users.AddRange(customers.Result);
            return users;
        }

        private async Task<List<Employee>> GetEmployeesByNameAndSurname(string name, string surname)
        {
            List<Employee> users = new List<Employee>();
            using (var context = new MyDbContext())
            {
                users = (await _employeesCache.FindAsync<Employee>(x => x.Name.ToLower() == name.ToLower() && x.Surname.ToLower() == surname.ToLower())).ToList();
            }
            return users;
        }

        private async Task<List<Customer>> GetCustomersByNameAndSurname(string name, string surname)
        {
            List<Customer> users = new List<Customer>();
            using (var context = new MyDbContext())
            {
                users = (await _customersCache.FindAsync<Customer>(x => x.Name.ToLower() == name.ToLower() && x.Surname.ToLower() == surname.ToLower())).ToList();
            }
            return users;
        }

        public async Task UpdateUser(User user)
        {
            if (user is Employee)
                await _employeesCache.ReplaceOneAsync<Employee>(x => x.Id == ((Employee)user).Id, (Employee)user);
            else
                await _customersCache.ReplaceOneAsync<Customer>(x => x.Id == ((Customer)user).Id, (Customer)user);
        }

        public async Task DeleteUser(User user)
        {
            if (user is Employee)
                await _employeesCache.DeleteOneAsync<Employee>(x => x.Id == ((Employee)user).Id);
            else
                await _customersCache.DeleteOneAsync<Customer>(x => x.Id == ((Customer)user).Id);
        }
    }
}
