﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Store.Core.Models;

namespace Store.Core.Contracts
{
    public interface IOrderCacheRepository
    {
        Task<List<Order>> GetAllOrders();
        Task<Order> AddNewOrder(Order order);
        Task AddOrders(List<Order> orders);
        Task<Order> GetOrderById(int id);
        Task<List<Order>> GetOrdersByEmployee(User user);
        Task<List<Order>> GetOrdersByCustomer(User user);
        Task<List<Order>> GetOrdersByProduct(Product product);
        Task UpdateOrder(Order order);
        Task DeleteOrder(Order order);
        Task DeleteCache();
        Task<int> GetOrderCount();
    }
}
