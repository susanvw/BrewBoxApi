using BrewBoxApi.Domain.Aggregates.Orders;
using BrewBoxApi.Presentation.Features.Orders.CreateOrderCommand;
using BrewBoxApi.Presentation.Features.Orders.Models;
using BrewBoxApi.Presentation.Features.Orders.UpdateOrderCommand;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrewBoxApi.Presentation.Features.Orders;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OrdersController(IOrderControllerImplementation implementation) : ControllerBase
{
    [HttpGet("{id}", Name = "GetOrderById")]
    [Authorize(Roles = "Barista, Customer")]
    public async Task<ActionResult<OrderView>> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var order = await implementation.GetByIdAsync(id, cancellationToken);
        return Ok(order);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderView>>> GetAllByUserIdAsync(CancellationToken cancellationToken = default)
    {
        var orders = await implementation.GetAllByUserIdAsync(cancellationToken);
        return Ok(orders);
    }

    [HttpGet("active")]
    [Authorize(Roles = "Customer")]
    public async Task<ActionResult<IEnumerable<OrderView>>> GetAllActiveByUserIdAsync(CancellationToken cancellationToken = default)
    {
        var orders = await implementation.GetAllActiveByUserIdAsync(cancellationToken);
        return Ok(orders);
    }

    [HttpGet("current")]
    [Authorize(Roles = "Barista")]
    public async Task<ActionResult<IEnumerable<OrderView>>> GetCurrentOrdersAsync(CancellationToken cancellationToken = default)
    {
        var orders = await implementation.GetCurrentOrdersAsync(cancellationToken);
        return Ok(orders);
    }

    [HttpPost]
    public async Task<ActionResult<string>> CreateAsync([FromBody] CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        var id = await implementation.AddAsync(request, cancellationToken);
        return CreatedAtRoute("GetOrderById", new { id }, id);
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Barista, Customer")]
    public async Task<IActionResult> UpdateStatusAsync(string id, [FromBody] UpdateOrderStatusRequest request, CancellationToken cancellationToken = default)
    {
        await implementation.UpdateAsync(id, request, cancellationToken);
        return NoContent();
    }
}