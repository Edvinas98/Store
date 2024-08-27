using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Store.Core.Contracts;
using Store.Core.Enums;
using Store.Core.Models;

namespace Store.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public UserController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                List<User> users = await _orderService.GetAllUsers();
                if (users.Count == 0)
                    return NotFound("There are no users registered");
                return Ok(users);
            }
            catch
            {
                return Problem();
            }
        }

        [HttpGet("GetEmployees")]
        public async Task<IActionResult> GetEmployees()
        {
            try
            {
                List<Employee> users = await _orderService.GetAllEmployees();
                if (users.Count == 0)
                    return NotFound("There are no employees registered");
                return Ok(users);
            }
            catch
            {
                return Problem();
            }
        }

        [HttpGet("GetCustomers")]
        public async Task<IActionResult> GetCustomers()
        {
            try
            {
                List<Customer> users = await _orderService.GetAllCustomers();
                if (users.Count == 0)
                    return NotFound("There are no customers registered");
                return Ok(users);
            }
            catch
            {
                return Problem();
            }
        }

        [HttpGet("GetEmployeeById")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            try
            {
                Employee user = await _orderService.GetEmployeeById(id);
                if (user.Id == 0)
                    return NotFound("Employee with this Id was not found");
                return Ok(user);
            }
            catch
            {
                return Problem();
            }
        }

        [HttpGet("GetCustomerById")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            try
            {
                Customer user = await _orderService.GetCustomerById(id);
                if (user.Id == 0)
                    return NotFound("Customer with this Id was not found");
                return Ok(user);
            }
            catch
            {
                return Problem();
            }
        }

        [HttpGet("GetUserByNameAndSurname")]
        public async Task<IActionResult> GetUserByNameAndSurname(string name, string surname)
        {
            try
            {
                List<User> users = await _orderService.GetUserByNameAndSurname(name, surname);
                if (users.Count() == 0)
                    return NotFound("User with this data was not found");
                return Ok(users);
            }
            catch
            {
                return Problem();
            }
        }

        [HttpPost("AddNewUser")]
        public async Task<IActionResult> AddNewUser(string name, string surname, string email, string phoneNumber, bool bEmployee)
        {
            try
            {
                User user = new User();
                if (bEmployee)
                    user = new Employee(0, name, surname, email, phoneNumber);
                else
                    user = new Customer(0, name, surname, email, phoneNumber);
                string result = await _orderService.AddNewUser(user);
                if (!string.IsNullOrEmpty(result))
                    return NotFound(result);
                return Ok("User was added successfully");
            }
            catch
            {
                return Problem();
            }
        }

        [HttpPatch("UpdateEmployee")]
        public async Task<IActionResult> UpdateEmployee(int id, string name, string surname, string email, string phoneNumber)
        {
            try
            {
                Employee user = await _orderService.GetEmployeeById(id);
                if (user.Name == string.Empty)
                    return NotFound("Employee with this Id was not found");
                user.Name = name;
                user.Surname = surname;
                user.Email = user.ValidateEmail(email);
                user.PhoneNumber = user.ValidatePhoneNumber(phoneNumber);
                string result = await _orderService.UpdateUser(user);
                if (!string.IsNullOrEmpty(result))
                    return NotFound(result);
                return Ok("Employee was updated successfully");
            }
            catch
            {
                return Problem();
            }
        }

        [HttpPatch("UpdateCustomer")]
        public async Task<IActionResult> UpdateCustomer(int id, string name, string surname, string email, string phoneNumber)
        {
            try
            {
                Customer user = await _orderService.GetCustomerById(id);
                if (user.Name == string.Empty)
                    return NotFound("Customer with this Id was not found");
                user.Name = name;
                user.Surname = surname;
                user.Email = user.ValidateEmail(email);
                user.PhoneNumber = user.ValidatePhoneNumber(phoneNumber);
                string result = await _orderService.UpdateUser(user);
                if (!string.IsNullOrEmpty(result))
                    return NotFound(result);
                return Ok("Customer was updated successfully");
            }
            catch
            {
                return Problem();
            }
        }

        [HttpDelete("DeleteEmployee")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                Employee user = await _orderService.GetEmployeeById(id);
                if (user.Name == string.Empty)
                    return NotFound("Employee with this Id was not found");
                await _orderService.DeleteUser(user);
                return Ok("Employee was deleted successfully");
            }
            catch
            {
                return Problem();
            }
        }

        [HttpDelete("DeleteCustomer")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            try
            {
                Customer user = await _orderService.GetCustomerById(id);
                if (user.Name == string.Empty)
                    return NotFound("Customer with this Id was not found");
                await _orderService.DeleteUser(user);
                return Ok("Customer was deleted successfully");
            }
            catch
            {
                return Problem();
            }
        }
    }
}
