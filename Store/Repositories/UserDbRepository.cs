using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Store.Core.Contracts;
using Store.Core.Models;

namespace Store.Core.Repositories
{
    public class UserDbRepository : IUserRepository
    {
        public async Task<int> GetEmployeeCount()
        {
            using (var context = new MyDbContext())
            {
                return (await context.Employees.ToListAsync()).Count();
            }
        }

        public async Task<int> GetCustomerCount()
        {
            using (var context = new MyDbContext())
            {
                return (await context.Customers.ToListAsync()).Count();
            }
        }

        public async Task<User> AddNewUser(User user)
        {
            using (var context = new MyDbContext())
            {
                if (user is Employee)
                    await context.Employees.AddAsync((Employee)user);
                else
                    await context.Customers.AddAsync((Customer)user);
                await context.SaveChangesAsync();
            }
            return user;
        }
        public async Task<string> CheckUser(User user)
        {
            if(user is Employee)
            {
                var emailEmployees = CheckEmployeesByEmail(user);
                var phoneEmployees = CheckEmployeesByPhoneNumber(user);
                await Task.WhenAll(emailEmployees, phoneEmployees);

                if (emailEmployees.Result.Count > 0)
                    return "A user with such email is already registered";
                if (phoneEmployees.Result.Count > 0)
                    return "A user with such phone number is already registered";
            }
            else
            {
                var emailCustomers = CheckCustomersByEmail(user);
                var phoneCustomers = CheckCustomersByPhoneNumber(user);
                await Task.WhenAll(emailCustomers, phoneCustomers);

                if (emailCustomers.Result.Count > 0)
                    return "A user with such email is already registered";
                if (phoneCustomers.Result.Count > 0)
                    return "A user with such phone number is already registered";
            }
            return string.Empty;
        }

        private async Task<List<Employee>> CheckEmployeesByEmail(User user)
        {
            using (var context = new MyDbContext())
            {
                return await context.Employees.Where<Employee>(x => x.Id != ((Employee)user).Id && x.Email == user.Email).ToListAsync();
            }
        }

        private async Task<List<Customer>> CheckCustomersByEmail(User user)
        {
            using (var context = new MyDbContext())
            {
                return await context.Customers.Where<Customer>(x => x.Id != ((Customer)user).Id && x.Email == user.Email).ToListAsync();
            }
        }

        private async Task<List<Employee>> CheckEmployeesByPhoneNumber(User user)
        {
            using (var context = new MyDbContext())
            {
                return await context.Employees.Where<Employee>(x => x.Id != ((Employee)user).Id && x.Id != ((Employee)user).Id && x.PhoneNumber == user.PhoneNumber).ToListAsync();
            }
        }

        private async Task<List<Customer>> CheckCustomersByPhoneNumber(User user)
        {
            using (var context = new MyDbContext())
            {
                return await context.Customers.Where<Customer>(x => x.Id != ((Customer)user).Id && x.Id != ((Customer)user).Id && x.PhoneNumber == user.PhoneNumber).ToListAsync();
            }
        }

        public async Task DeleteUser(User user)
        {
            using (var context = new MyDbContext())
            {
                if (user is Employee)
                    context.Employees.Remove((Employee)user);
                else
                    context.Customers.Remove((Customer)user);
                await context.SaveChangesAsync();
            }
        }

        public async Task<List<User>> GetAllUsers()
        {
            List<User> users = new List<User>();
            var employees = GetAllEmployees();
            var customers = GetAllCustomers();
            await Task.WhenAll(employees, customers);
            users.AddRange(employees.Result);
            users.AddRange(customers.Result);
            return users;
        }

        public async Task<List<Employee>> GetAllEmployees()
        {
            List<Employee> users = new List<Employee>();
            using (var context = new MyDbContext())
            {
                users = await context.Employees.ToListAsync();
            }
            return users;
        }

        public async Task<List<Customer>> GetAllCustomers()
        {
            List<Customer> users = new List<Customer>();
            using (var context = new MyDbContext())
            {
                users = await context.Customers.ToListAsync();
            }
            return users;
        }

        public async Task<Employee> GetEmployeeById(int id)
        {
            using (var context = new MyDbContext())
            {
                List<Employee> users = await context.Employees.Where(x => x.Id == id).ToListAsync();
                if (users.Count() > 0)
                    return users[0];
            }
            return new Employee();
        }

        public async Task<Customer> GetCustomerById(int id)
        {
            using (var context = new MyDbContext())
            {
                List<Customer> users = await context.Customers.Where(x => x.Id == id).ToListAsync();
                if (users.Count() > 0)
                    return users[0];
            }
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
                users = await context.Employees.Where(x => x.Name.ToLower() == name.ToLower() && x.Surname.ToLower() == surname.ToLower()).ToListAsync();
            }
            return users;
        }

        private async Task<List<Customer>> GetCustomersByNameAndSurname(string name, string surname)
        {
            List<Customer> users = new List<Customer>();
            using (var context = new MyDbContext())
            {
                users = await context.Customers.Where(x => x.Name.ToLower() == name.ToLower() && x.Surname.ToLower() == surname.ToLower()).ToListAsync();
            }
            return users;
        }

        public async Task UpdateUser(User user)
        {
            using (var context = new MyDbContext())
            {
                if (user is Employee)
                    context.Employees.Update((Employee)user);
                else
                    context.Customers.Update((Customer)user);
                await context.SaveChangesAsync();
            }
        }
    }
}
