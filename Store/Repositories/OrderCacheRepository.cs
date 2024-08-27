using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using Store.Core.Contracts;
using Store.Core.Models;

namespace Store.Core.Repositories
{
    public class OrderCacheRepository : IOrderCacheRepository
    {
        private readonly IMongoCollection<Order> _ordersCache;

        public OrderCacheRepository(IMongoClient mongoClient)
        {
            _ordersCache = mongoClient.GetDatabase("store").GetCollection<Order>("orders_cache");
        }

        public Task DeleteCache()
        {
            _ordersCache.Database.DropCollectionAsync("orders_cache");
            return Task.CompletedTask;
        }

        public async Task<int> GetOrderCount()
        {
            return (await _ordersCache.FindAsync<Order>(x => x.Id != 0)).ToList().Count();
        }

        public async Task<Order> AddNewOrder(Order order)
        {
            await _ordersCache.InsertOneAsync(order);
            return order;
        }

        public async Task AddOrders(List<Order> orders)
        {
            if (orders.Count > 0)
            {
                await _ordersCache.Database.DropCollectionAsync("orders_cache");
                await _ordersCache.InsertManyAsync(orders);
            }
        }

        public async Task DeleteOrder(Order order)
        {
            await _ordersCache.DeleteOneAsync<Order>(x => x.Id == order.Id);
        }

        public async Task<List<Order>> GetAllOrders()
        {
            return (await _ordersCache.FindAsync<Order>(x => x.Id != 0)).ToList();
        }

        public async Task<Order> GetOrderById(int id)
        {
            List<Order> orders = (await _ordersCache.FindAsync<Order>(x => x.Id == id)).ToList();
            if (orders.Count() > 0)
                return orders[0];
            return new Order();
        }

        public async Task<List<Order>> GetOrdersByProduct(Product product)
        {
            return (await _ordersCache.FindAsync<Order>(x => x.OrderProduct.Id == product.Id)).ToList();
        }

        public async Task<List<Order>> GetOrdersByEmployee(User user)
        {
            return (await _ordersCache.FindAsync<Order>(x => x.Employee.Id == ((Employee)user).Id)).ToList();
        }

        public async Task<List<Order>> GetOrdersByCustomer(User user)
        {
            return (await _ordersCache.FindAsync<Order>(x => x.Customer.Id == ((Customer)user).Id)).ToList();
        }

        public async Task UpdateOrder(Order order)
        {
            await _ordersCache.ReplaceOneAsync<Order>(x => x.Id == order.Id, order);
        }
    }
}
