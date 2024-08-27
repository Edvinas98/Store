using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Store.Core.Contracts;
using Store.Core.Enums;
using Store.Core.Models;

namespace Store.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("GetOrders")]
        public async Task<IActionResult> GetOrders()
        {
            try
            {
                List<Order> orders = await _orderService.GetAllOrders();
                if (orders.Count == 0)
                    return NotFound("There are no orders registered");
                return Ok(orders);
            }
            catch
            {
                return Problem();
            }
        }

        [HttpGet("GetOrderById")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            try
            {
                Order order = await _orderService.GetOrderById(id);
                if (order.Id == 0)
                    return NotFound("Order with this Id was not found");
                return Ok(order);
            }
            catch
            {
                return Problem();
            }
        }

        [HttpGet("GetOrderByEmployee")]
        public async Task<IActionResult> GetOrderByEmployee(int id)
        {
            try
            {
                Employee user = await _orderService.GetEmployeeById(id);
                if (user.Id == 0)
                    return NotFound("Employee with this Id was not found");
                List<Order> orders = await _orderService.GetOrdersByEmployee(user);
                if (orders.Count == 0)
                    return NotFound("There are no orders registered by this employee");
                return Ok(orders);
            }
            catch
            {
                return Problem();
            }
        }

        [HttpGet("GetOrderByCustomer")]
        public async Task<IActionResult> GetOrderByCustomer(int id)
        {
            try
            {
                Customer user = await _orderService.GetCustomerById(id);
                if (user.Id == 0)
                    return NotFound("Customer with this Id was not found");

                List<Order> orders = await _orderService.GetOrdersByCustomer(user);
                if (orders.Count == 0)
                    return NotFound("There are no orders registered for this customer");
                return Ok(orders);
            }
            catch
            {
                return Problem();
            }
        }

        [HttpGet("GetOrderByProduct")]
        public async Task<IActionResult> GetOrderByProduct(int id)
        {
            try
            {
                Product product = await _orderService.GetProductById(id);
                if (product.Id == 0)
                    return NotFound("Product with this Id was not found");

                List<Order> orders = await _orderService.GetOrdersByProduct(product);
                if (orders.Count == 0)
                    return NotFound("There are no orders registered with this product");
                return Ok(orders);
            }
            catch
            {
                return Problem();
            }
        }

        [HttpGet("GetOrderPrice")]
        public async Task<IActionResult> GetOrderPrice(int id)
        {
            try
            {
                Order order = await _orderService.GetOrderById(id);
                if (order.Id == 0)
                    return NotFound("Order with this Id was not found");

                return Ok($"Order price: {await _orderService.GetOrderPrice(order)}");
            }
            catch
            {
                return Problem();
            }
        }

        [HttpPost("AddNewOrder")]
        public async Task<IActionResult> AddNewOrder(int employeeId, int customerId, int productId, int amount)
        {
            try
            {
                Employee employee = await _orderService.GetEmployeeById(employeeId);
                if (employee.Id == 0)
                    return NotFound("Employee with this Id was not found");

                Customer customer = await _orderService.GetCustomerById(customerId);
                if (customer.Id == 0)
                    return NotFound("Customer with this Id was not found");

                Product product = await _orderService.GetProductById(productId);
                if (product.Id == 0)
                    return NotFound("Product with this Id was not found");

                Order order = new Order(0, (Employee)employee, (Customer)customer, product, DateOnly.FromDateTime(DateTime.Now), amount);
                string result = await _orderService.AddNewOrder(order);
                if (!string.IsNullOrEmpty(result))
                    return NotFound(result);
                return Ok("Order was added successfully");
            }
            catch
            {
                return Problem();
            }
        }

        [HttpPatch("UpdateOrder")]
        public async Task<IActionResult> UpdateProduct(int id, int employeeId, int customerId, int productId, int amount)
        {
            try
            {
                Order order = await _orderService.GetOrderById(id);
                if (order.Id == 0)
                    return NotFound("Order with this Id was not found");

                Employee employee = await _orderService.GetEmployeeById(employeeId);
                if (employee.Id == 0)
                    return NotFound("Employee with this Id was not found");

                Customer customer = await _orderService.GetCustomerById(customerId);
                if (customer.Id == 0)
                    return NotFound("Customer with this Id was not found");

                Product product = await _orderService.GetProductById(productId);
                if (product.Id == 0)
                    return NotFound("Product with this Id was not found");

                order.Employee = (Employee)employee;
                order.Customer = (Customer)customer;
                order.OrderProduct = product;
                order.Amount = amount;

                string result = await _orderService.UpdateOrder(order);
                if (!string.IsNullOrEmpty(result))
                    return NotFound(result);
                return Ok("Order was updated successfully");
            }
            catch
            {
                return Problem();
            }
        }

        [HttpDelete("DeleteProduct")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                Order order = await _orderService.GetOrderById(id);
                if (order.Id == 0)
                    return NotFound("Order with this Id was not found");
                await _orderService.DeleteOrder(order);
                return Ok("Order was deleted successfully");
            }
            catch
            {
                return Problem();
            }
        }
    }
}
