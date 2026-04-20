using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Models;
using OrderService.Services;
using OrderService.Repository;
using static OrderService.Repository.IOrderRepository;

namespace OrderService.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderRepository _repository;
    private readonly ProductCatalogService _productCatalogService;
    private readonly ILogger<OrderController> _logger;

    public OrderController(IOrderRepository repository, ProductCatalogService productCatalogService, ILogger<OrderController> logger)
    {
        _repository = repository;
        _productCatalogService = productCatalogService;
        _logger = logger;
    }

    // Requirement 1 & 3
    [HttpGet("by-date")]
    public async Task<IActionResult> GetOrdersByDateRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        _logger.LogInformation("Henter ordrer fra {StartDate} til {EndDate}", startDate, endDate);

        if (startDate > endDate)
        {
            _logger.LogWarning("Ugyldigt datointerval: startdato {StartDate} er efter slutdato {EndDate}", startDate, endDate);
            return BadRequest("Startdato må ikke være efter slutdato.");
        }

        var orders = await _repository.GetAllOrdersFromASpecificTimeFrame(startDate, endDate);
        _logger.LogInformation("Returnerer {Count} ordrer for datointerval {StartDate} - {EndDate}", orders.Count(), startDate, endDate);
        return Ok(orders);
    }

    // Requirement 2 & 3
    [HttpGet("today")]
    public async Task<IActionResult> GetTodaysOrders()
    {
        _logger.LogInformation("Henter dagens ordrer for dato {Date}", DateTime.UtcNow.Date);

        var orders = await _repository.GetOrdersFromToday();
        _logger.LogInformation("Returnerer {Count} ordrer for i dag", orders.Count());
        return Ok(orders);
    }

    // Requirement 4 & 5
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] Order? order)
    {
        _logger.LogInformation("Forsøger at oprette ny ordre");

        if (order == null)
        {
            _logger.LogWarning("CreateOrder kaldt med null ordre");
            return BadRequest("Order må ikke være null.");
        }

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("CreateOrder kaldt med ugyldigt ModelState: {Errors}",
                string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
            return BadRequest(ModelState);
        }

        foreach (var item in order.Items)
        {
            _logger.LogDebug("Slår produkt op med id '{ProductId}' i varekataloget", item.ProductId);
            var product = await _productCatalogService.GetProductByIdAsync(item.ProductId);
            if (product == null)
            {
                _logger.LogWarning("Produkt med id '{ProductId}' blev ikke fundet i varekataloget", item.ProductId);
                return NotFound($"Produkt med id '{item.ProductId}' findes ikke i varekataloget.");
            }

            item.Price = product.Price;
            item.Name = product.Name;
        }

        order.OrderId = Guid.NewGuid();
        order.OrderStartdate = DateTime.UtcNow;
        order.TotalPrice = (int)order.Items.Sum(i => i.Price * i.Quantity);

        await _repository.CreateOrder(order);
        _logger.LogInformation("Ordre oprettet med id {OrderId} og totalpris {TotalPrice}", order.OrderId, order.TotalPrice);
        return CreatedAtAction(nameof(GetOrder), new { guid = order.OrderId }, order);
    }

    [HttpGet("{guid:guid}")]
    public async Task<IActionResult> GetOrder(Guid guid)
    {
        _logger.LogInformation("Henter ordre med id {OrderId}", guid);

        var order = await _repository.GetOrder(guid);
        if (order == null)
        {
            _logger.LogWarning("Ordre med id {OrderId} blev ikke fundet", guid);
            return NotFound();
        }

        return Ok(order);
    }

    [HttpDelete("{guid:guid}")]
    public async Task<IActionResult> DeleteOrder(Guid guid)
    {
        _logger.LogInformation("Sletter ordre med id {OrderId}", guid);
        await _repository.DeleteOrder(guid);
        _logger.LogInformation("Ordre med id {OrderId} blev slettet", guid);
        return NoContent();
    }
}