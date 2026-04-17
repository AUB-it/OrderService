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

        public OrderController(IOrderRepository repository, ProductCatalogService productCatalogService)
        {
            _repository = repository;
            _productCatalogService = productCatalogService;
        }

        // Requirement 1 & 3
        [HttpGet("by-date")]
        public async Task<IActionResult> GetOrdersByDateRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            if (startDate > endDate)
                return BadRequest("Startdato må ikke være efter slutdato.");

            var orders = await _repository.GetAllOrdersFromASpecificTimeFrame(startDate, endDate);
            return Ok(orders);
        }

        // Requirement 2 & 3
        [HttpGet("today")]
        public async Task<IActionResult> GetTodaysOrders()
        {
            var orders = await _repository.GetOrdersFromToday();
            return Ok(orders);
        }

        // Requirement 4 & 5
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] Order? order)
        {
            if (order == null)
                return BadRequest("Order må ikke være null.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            foreach (var item in order.Items)
            {
                var product = await _productCatalogService.GetProductByIdAsync(item.ProductId);
                if (product == null)
                    return NotFound($"Produkt med id '{item.ProductId}' findes ikke i varekataloget.");

                item.Price = product.Price;
                item.Name = product.Name;
            }

            order.OrderId = Guid.NewGuid();
            order.OrderStartdate = DateTime.UtcNow;
            order.TotalPrice = (int)order.Items.Sum(i => i.Price * i.Quantity);

            await _repository.CreateOrder(order);
            return CreatedAtAction(nameof(GetOrder), new { guid = order.OrderId }, order);
        }

        [HttpGet("{guid:guid}")]
        public async Task<IActionResult> GetOrder(Guid guid)
        {
            var order = await _repository.GetOrder(guid);
            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [HttpDelete("{guid:guid}")]
        public async Task<IActionResult> DeleteOrder(Guid guid)
        {
            await _repository.DeleteOrder(guid);
            return NoContent();
        }
    }