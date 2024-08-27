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
    public class OrderDbRepository : IOrderRepository
    {
        public async Task<int> GetOrderCount()
        {
            using (var context = new MyDbContext())
            {
                return (await context.Orders.ToListAsync()).Count();
            }
        }
        public async Task<Order> AddNewOrder(Order order)
        {
            using (var context = new MyDbContext())
            {
                order.Employee = await context.Employees.Where(x => x.Id == order.Employee.Id).FirstAsync();
                order.Customer = await context.Customers.Where(x => x.Id == order.Customer.Id).FirstAsync();
                order.OrderProduct = await context.Products.Where(x => x.Id == order.OrderProduct.Id).FirstAsync();
                await context.Orders.AddAsync(order);
                await context.SaveChangesAsync();
            }
            return order;
        }

        public async Task DeleteOrder(Order order)
        {
            using (var context = new MyDbContext())
            {
                context.Orders.Remove(order);
                await context.SaveChangesAsync();
            }
        }

        public async Task<List<Order>> GetAllOrders()
        {
            List<Order> orders = new List<Order>();
            using (var context = new MyDbContext())
            {
                orders = await context.Orders.Include(x => x.Employee).Include(x => x.Customer).Include(x => x.OrderProduct).ToListAsync();
            }
            return orders;
        }

        public async Task<Order> GetOrderById(int id)
        {
            using (var context = new MyDbContext())
            {
                List<Order> orders = await context.Orders.Include(x => x.Employee).Include(x => x.Customer).Include(x => x.OrderProduct).Where(x => x.Id == id).ToListAsync();
                if (orders.Count > 0)
                    return orders[0];
            }
            return new Order();
        }

        public async Task<List<Order>> GetOrdersByProduct(Product product)
        {
            List<Order> orders = new List<Order>();
            using (var context = new MyDbContext())
            {
                orders = await context.Orders.Include(x => x.Employee).Include(x => x.Customer).Include(x => x.OrderProduct).Where(x => x.OrderProduct == product).ToListAsync();
            }
            return orders;
        }

        public async Task<List<Order>> GetOrdersByEmployee(User user)
        {
            List<Order> orders = new List<Order>();
            using (var context = new MyDbContext())
            {
                orders = await context.Orders.Include(x => x.Employee).Include(x => x.Customer).Include(x => x.OrderProduct).Where(x => x.Employee == user).ToListAsync();
            }
            return orders;
        }

        public async Task<List<Order>> GetOrdersByCustomer(User user)
        {
            List<Order> orders = new List<Order>();
            using (var context = new MyDbContext())
            {
                orders = await context.Orders.Include(x => x.Employee).Include(x => x.Customer).Include(x => x.OrderProduct).Where(x => x.Customer == user).ToListAsync();
            }
            return orders;
        }

        public async Task UpdateOrder(Order order)
        {
            using (var context = new MyDbContext())
            {
                context.Orders.Update(order);
                await context.SaveChangesAsync();
            }
        }
    }
}
