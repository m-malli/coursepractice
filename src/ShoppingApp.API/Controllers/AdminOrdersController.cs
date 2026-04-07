using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Application.Interfaces;

namespace ShoppingApp.API.Controllers;

[ApiController]
[Route("api/admin/orders")]
[Authorize(Roles = "Admin")]
public class AdminOrdersController : ControllerBase
{
    private readonly IOrderService _orders;
    public AdminOrdersController(IOrderService orders) => _orders = orders;

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] string status)
    {
        var result = await _orders.UpdateStatusAsync(id, status);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }
}
