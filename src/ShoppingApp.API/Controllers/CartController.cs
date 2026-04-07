using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Application.DTOs.Cart;
using ShoppingApp.Application.Interfaces;

namespace ShoppingApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _cart;
    public CartController(ICartService cart) => _cart = cart;

    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var result = await _cart.GetCartAsync(UserId);
        return Ok(result.Data);
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItem(AddToCartDto dto)
    {
        var result = await _cart.AddToCartAsync(UserId, dto);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }

    [HttpPatch("items/{productId:guid}")]
    public async Task<IActionResult> UpdateItem(Guid productId, UpdateCartItemDto dto)
    {
        var result = await _cart.UpdateQuantityAsync(UserId, productId, dto);
        return result.Success ? Ok(result.Data) : NotFound(new { error = result.Error });
    }

    [HttpDelete("items/{productId:guid}")]
    public async Task<IActionResult> RemoveItem(Guid productId)
    {
        await _cart.RemoveFromCartAsync(UserId, productId);
        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> Clear()
    {
        await _cart.ClearCartAsync(UserId);
        return NoContent();
    }
}
