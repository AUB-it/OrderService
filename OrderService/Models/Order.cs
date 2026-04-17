namespace OrderService.Models;


public class Order
{
    public Guid OrderId  { get; set; } //id tælder som navn
    public DateTime OrderStartdate { get; set; } //oprettelsesdato
    public DateTime OrderEndDate  { get; set; } //order slut dato
    public List<Orderitem> Items { get; set; } //liste af produkters id
    public int UserId { get; set; } //Hvem er order til
    public int TotalPrice { get; set; } //samlet pris for alle varene
}
public class Orderitem
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string Name { get; set; }
}