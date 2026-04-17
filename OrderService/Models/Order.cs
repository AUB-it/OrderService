namespace OrderService.Models;


public class Order
{
    public Guid Orderid  { get; set; } //id tælder som navn
    public DateTime OrderStartdate { get; set; } //oprettelsesdato
    public DateTime OrderEndDate  { get; set; } //order slut dato
    public List<Orderitem> Items { get; set; } //liste af produkters id
    public int Touserid { get; set; } //Hvem er order til
    public int TotalPrice { get; set; } //samlet pris for alle varene
}
public class Orderitem
{
    public Guid Id { get; set; }
    public int amount { get; set; }
    public decimal price { get; set; }
    public string Name { get; set; }
}