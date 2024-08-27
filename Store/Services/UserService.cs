using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Store.Core.Contracts;
using Store.Core.Enums;
using Store.Core.Models;
using Store.Core.Repositories;

namespace Store.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserCacheRepository _userCacheRepository;
        private readonly ILogger _logger;

        public UserService(IUserRepository userRepository, IUserCacheRepository userCacheRepository, ILogger logger)
        {
            _userRepository = userRepository;
            _userCacheRepository = userCacheRepository;
            _logger = logger;
            _logger.Information("User service started");
        }

        public async Task DeleteCache()
        {
            while (true)
            {
                await Task.Delay(2 * 60000);
                var deleteCache = _userCacheRepository.DeleteCache();
            }
        }

        private async Task<bool> VerifyEmployeeCache()
        {
            var databaseCount = _userRepository.GetEmployeeCount();
            var cacheCount = _userCacheRepository.GetEmployeeCount();
            await Task.WhenAll(databaseCount, cacheCount);
            if (databaseCount.Result == cacheCount.Result)
                return true;
            return false;
        }

        private async Task<bool> VerifyCustomerCache()
        {
            var databaseCount = _userRepository.GetCustomerCount();
            var cacheCount = _userCacheRepository.GetCustomerCount();
            await Task.WhenAll(databaseCount, cacheCount);
            if (databaseCount.Result == cacheCount.Result)
                return true;
            return false;
        }

        private async Task<bool> VerifyCache()
        {
            var dbEmployeeCount = _userRepository.GetEmployeeCount();
            var cacheEmployeeCount = _userCacheRepository.GetEmployeeCount();
            var dbCustomerCount = _userRepository.GetCustomerCount();
            var cacheCustomerCount = _userCacheRepository.GetCustomerCount();
            await Task.WhenAll(dbEmployeeCount, cacheEmployeeCount, dbCustomerCount, cacheCustomerCount);
            if (dbEmployeeCount.Result == cacheEmployeeCount.Result && dbCustomerCount.Result == cacheCustomerCount.Result)
                return true;
            return false;
        }

        public async Task<List<User>> GetAllUsers()
        {
            _logger.Information("'GetAllUsers' was called");
            List<User> result = new List<User>();
            if (await VerifyCache())
            {
                _logger.Information("Loading users from cache");
                result = await _userCacheRepository.GetAllUsers();
            }
            else
            {
                _logger.Information("Loading users from SQL");
                result = await _userRepository.GetAllUsers();
                await _userCacheRepository.AddUsers(result);
            }
            _logger.Information($"Loaded {result.Count()} users");
            return result;
        }

        public async Task<List<Employee>> GetAllEmployees()
        {
            List<Employee> result = new List<Employee>();

            _logger.Information("'GetAllEmployees' was called");
            if (await VerifyEmployeeCache())
            {
                _logger.Information("Loading employees from cache");
                result = await _userCacheRepository.GetAllEmployees();
            }
            else
            {
                _logger.Information("Loading employees from SQL");
                result = await _userRepository.GetAllEmployees();
            }
            _logger.Information($"Loaded {result.Count()} employees");
            return result;
        }

        public async Task<List<Customer>> GetAllCustomers()
        {
            List<Customer> result = new List<Customer>();
            _logger.Information("'GetAllCustomers' was called");
            if (await VerifyCustomerCache())
            {
                _logger.Information("Loading customers from cache");
                result = await _userCacheRepository.GetAllCustomers();
            }
            else
            {
                _logger.Information("Loading customers from SQL");
                result = await _userRepository.GetAllCustomers();
            }
            _logger.Information($"Loaded {result.Count()} customers");
            return result;
        }

        public async Task<string> AddNewUser(User user)
        {
            _logger.Information($"'AddNewUser' was called, user id: {user.Id}");
            string checkResult = (await CheckUserData(user));
            if (!string.IsNullOrEmpty(checkResult))
            {
                _logger.Warning($"user adding aborted - {checkResult}");
                return checkResult;
            }

            if (await VerifyCache())
                checkResult = await _userCacheRepository.CheckUser(user);
            else
                checkResult = await _userRepository.CheckUser(user);

            if (string.IsNullOrEmpty(checkResult))
            {
                _logger.Information("User was added successfully");
                await _userCacheRepository.AddNewUser(await _userRepository.AddNewUser(user));
            }
            else
                _logger.Warning($"user adding aborted - {checkResult}");
            return checkResult;
        }

        public async Task<Employee> GetEmployeeById(int id)
        {
            _logger.Information($"'GetEmployeeById' was called, Id: {id}");
            Employee user = await _userCacheRepository.GetEmployeeById(id);
            if (user.Id != 0)
                return user;
            user = await _userRepository.GetEmployeeById(id);
            if (user.Id != 0)
                await _userCacheRepository.AddNewUser(user);
            _logger.Information($"Returning employee with Id: {user.Id}");
            return user;
        }

        public async Task<Customer> GetCustomerById(int id)
        {
            _logger.Information($"'GetCustomerById' was called, Id: {id}");
            Customer user = await _userCacheRepository.GetCustomerById(id);
            if (user.Id != 0)
                return user;
            user = await _userRepository.GetCustomerById(id);
            if (user.Id != 0)
                await _userCacheRepository.AddNewUser(user);
            _logger.Information($"Returning customer with Id: {user.Id}");
            return user;
        }

        public async Task<List<User>> GetUserByNameAndSurname(string name, string surname)
        {
            _logger.Information($"'GetUserByNameAndSurname' was called, name: {name}, surname: {surname}");
            List<User> users = new List<User>();
            if (await VerifyCache())
                users = await _userCacheRepository.GetUsersByNameAndSurname(name, surname);
            else
            {
                users = await _userRepository.GetUsersByNameAndSurname(name, surname);
            }
            _logger.Information($"Returning {users.Count()} users");
            return users;
        }

        public async Task<string> UpdateUser(User user)
        {
            _logger.Information($"'UpdateUser' was called, user id: {user.Id}");
            string checkResult = await CheckUserData(user);
            if (!string.IsNullOrEmpty(checkResult))
            {
                _logger.Warning($"Aborting, reason: {checkResult}");
                return checkResult;
            }

            if (await VerifyCache())
                checkResult = await _userCacheRepository.CheckUser(user);
            else
                checkResult = await _userRepository.CheckUser(user);

            if (string.IsNullOrEmpty(checkResult))
            {
                _logger.Information("User update was successful");
                var updateDb = _userRepository.UpdateUser(user);
                var updateCache = _userCacheRepository.UpdateUser(user);
                await Task.WhenAll(updateDb, updateCache);
            }
            else
                _logger.Warning($"Aborting, reason: {checkResult}");
            return checkResult;
        }

        public async Task DeleteUser(User user)
        {
            _logger.Information($"'DeleteUser' was called, user id: {user.Id}");
            var updateDb = _userRepository.DeleteUser(user);
            var updateCache = _userCacheRepository.DeleteUser(user);
            await Task.WhenAll(updateDb, updateCache);
        }

        private Task<string> CheckUserData(User user)
        {
            if (string.IsNullOrEmpty(user.Name))
                return Task.FromResult("Wrong name format detected");
            if (string.IsNullOrEmpty(user.Surname))
                return Task.FromResult("Wrong surname format detected");
            if (string.IsNullOrEmpty(user.Email))
                return Task.FromResult("Wrong email format detected");
            if (string.IsNullOrEmpty(user.PhoneNumber))
                return Task.FromResult("Wrong phone number format detected");
            return Task.FromResult(string.Empty);
        }
    }
}
