using System;
using OrderService.Models;
using OrderService.Repository;
using MongoDB.Bson;
using MongoDB.Driver;


namespace OrderService.Repository;

public class OrderRepository : IOrderRepository
{
    private readonly IMongoCollection<Order> _orderCollection;

    public OrderRepository(IMongoCollection<Order> orderCollection)
    {
        _orderCollection = orderCollection;
    }

    public async Task CreateOrder(Order order)
    {
        // Ensure the order has a unique ID (create a new one if not set)
        if (order.Orderid == Guid.Empty)
        {
            order.Orderid = Guid.NewGuid();
        }

        await _orderCollection.InsertOneAsync(order);
    }

    public async Task DeleteOrder(Guid guid)
    {
        await _orderCollection.DeleteOneAsync(order => order.Orderid == guid);
    }

    public async Task<Order> GetOrder(Guid guid)
    {
        return await _orderCollection.Find(order => order.Orderid == guid).FirstOrDefaultAsync();
    }

    public async Task<List<Order>> GetOrders()
    {
        return await _orderCollection.Find(order => true).ToListAsync();
    }

    public async Task<List<Order>> GetAllOrdersFromASpecificTimeFrame(DateTime OrderStartdate, DateTime OrderEndsdate)
    {
        return await _orderCollection.Find(order => 
            order.OrderStartdate >= OrderStartdate && 
            order.OrderEndDate <= OrderEndsdate)
            .ToListAsync();
    }

    public async Task<List<Order>> GetOrdersFromToday()
    {
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);
        
        return await _orderCollection.Find(order => 
            order.OrderStartdate >= today && 
            order.OrderStartdate < tomorrow)
            .ToListAsync();
    }
}