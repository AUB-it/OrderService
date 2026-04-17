using System;
using OrderService.Models;
using OrderService.Repository.IOrderRepository;
using MongoDB.Bson;
using MongoDB.Driver;


namespace OrderService.Repository;

public class OrderRepository : IOrderRepository.IOrderRepository
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

    public async Task DeleteOrder(int guid)
    {
        // Convert int to Guid for MongoDB query
        await _orderCollection.DeleteOneAsync(order => order.Orderid == new Guid(guid.ToString().PadLeft(32, '0')));
    }

    public async Task<Order> GetOrder(int guid)
    {
        // Convert int to Guid for MongoDB query
        return await _orderCollection.Find(order => order.Orderid == new Guid(guid.ToString().PadLeft(32, '0'))).FirstOrDefaultAsync();
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