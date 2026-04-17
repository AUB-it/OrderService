using OrderService.Models;

namespace OrderService.Repository.IOrderRepository;

public interface IOrderRepository
{
    Task CreateOrder(Order order); //oprettelse af order
    Task<List<Order>> GetOrders(); //henter alle order
    Task<Order> GetOrder(int guid); //henter en specifik order
    Task DeleteOrder(int guid); //slette en bruger
    Task<List<Order>> GetOrdersFromToday(); //muligt at hente order fra idag
    Task<List<Order>> GetAllOrdersFromASpecificTimeFrame(DateTime OrderStartdate, DateTime OrderEndsdate); //Muligt at hente ordrer fra en specifik timeframe
}