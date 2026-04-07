using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Application.DTOs.Orders;
using ShoppingApp.Application.Interfaces;

namespace ShoppingApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orders;
    public OrdersController(IOrderService orders) => _orders = orders;

    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderDto dto)
    {
        var result = await _orders.CreateFromCartAsync(UserId, dto);
        return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data)
            : BadRequest(new { error = result.Error });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _orders.GetByIdAsync(id, UserId);
        return result.Success ? Ok(result.Data) : NotFound(new { error = result.Error });
    }

    [HttpGet]
    public async Task<IActionResult> GetMyOrders()
    {
        var result = await _orders.GetUserOrdersAsync(UserId);
        return Ok(result.Data);
    }

    [HttpPatch("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var result = await _orders.CancelOrderAsync(id, UserId);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }
}
