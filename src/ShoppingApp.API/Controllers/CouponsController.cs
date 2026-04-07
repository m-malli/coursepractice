using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Application.Interfaces;

namespace ShoppingApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CouponsController : ControllerBase
{
    private readonly ICouponService _coupons;
    public CouponsController(ICouponService coupons) => _coupons = coupons;

    [HttpGet("validate/{code}")]
    public async Task<IActionResult> Validate(string code, [FromQuery] decimal orderTotal)
    {
        var result = await _coupons.ValidateAsync(code, orderTotal);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }
}
