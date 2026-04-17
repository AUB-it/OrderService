using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OrderService.Models;

public class Order
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid OrderId { get; set; }

    public DateTime OrderStartdate { get; set; }
    public DateTime OrderEndDate { get; set; }
    public List<Orderitem> Items { get; set; }
    public int UserId { get; set; }
    public int TotalPrice { get; set; }
}

public class Orderitem
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string Name { get; set; }
}