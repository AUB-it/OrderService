using OrderService.Models;

namespace OrderService.Repository;

public interface IOrderRepository
{
    Task CreateOrder(Order order); //oprettelse af order
    Task<List<Order>> GetOrders(); //henter alle order
    Task<Order> GetOrder(Guid OrderId); //henter en specifik order
    Task DeleteOrder(Guid OrderId); //slette en bruger
    Task<List<Order>> GetOrdersFromToday(); //muligt at hente order fra idag
    Task<List<Order>> GetAllOrdersFromASpecificTimeFrame(DateTime OrderStartdate, DateTime OrderEndsdate); //Muligt at hente ordrer fra en specifik timeframe
}