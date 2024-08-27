using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Store.Core.Contracts;
using Store.Core.Enums;
using Store.Core.Models;

namespace Store.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public ProductController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("GetProducts")]
        public async Task<IActionResult> GetProducts()
        {
            try
            {
                List<Product> products = await _orderService.GetAllProducts();
                if (products.Count == 0)
                    return NotFound("There are no products registered");
                return Ok(products);
            }
            catch
            {
                return Problem();
            }
        }

        [HttpGet("GetProductById")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                Product product = await _orderService.GetProductById(id);
                if (product.Id == 0)
                    return NotFound("Product with this Id was not found");
                return Ok(product);
            }
            catch
            {
                return Problem();
            }
        }

        [HttpGet("GetProductByName")]
        public async Task<IActionResult> GetProductByName(string name)
        {
            try
            {
                Product product = await _orderService.GetProductByName(name);
                if (product.Id == 0)
                    return NotFound("Product with this data was not found");
                return Ok(product);
            }
            catch
            {
                return Problem();
            }
        }

        [HttpPost("AddNewProduct")]
        public async Task<IActionResult> AddNewProduct(string name, decimal price, int stock, ProductCategory category)
        {
            try
            {
                Product product = new Product(0, name, price, stock, category);
                string result = await _orderService.AddNewProduct(product);
                if (!string.IsNullOrEmpty(result))
                    return NotFound(result);
                return Ok("Product was added successfully");
            }
            catch
            {
                return Problem();
            }
        }

        [HttpPost("AddProductStock")]
        public async Task<IActionResult> AddProductStock(int id, int amount)
        {
            try
            {
                Product product = await _orderService.GetProductById(id);
                if (product.Id == 0)
                    return NotFound("Product with this Id was not found");
                string result = await _orderService.AddStockToProduct(product, amount);
                if (!string.IsNullOrEmpty(result))
                    return NotFound(result);

                return Ok("Product stock was added successfully");
            }
            catch
            {
                return Problem();
            }
        }

        [HttpPatch("UpdateProduct")]
        public async Task<IActionResult> UpdateProduct(int id, string name, decimal price, int stock, ProductCategory category)
        {
            try
            {
                Product product = await _orderService.GetProductById(id);
                if (product.Id == 0)
                    return NotFound("Product with this Id was not found");
                product.Name = name;
                product.Price = Math.Floor(price * 100M) * 0.01M;
                product.Stock = stock;
                product.Category = category;
                string result = await _orderService.UpdateProduct(product);
                if (!string.IsNullOrEmpty(result))
                    return NotFound(result);
                return Ok("Product was updated successfully");
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
                Product product = await _orderService.GetProductById(id);
                if (product.Id == 0)
                    return NotFound("Product with this Id was not found");
                await _orderService.DeleteProduct(product);
                return Ok("Product was deleted successfully");
            }
            catch
            {
                return Problem();
            }
        }
    }
}
