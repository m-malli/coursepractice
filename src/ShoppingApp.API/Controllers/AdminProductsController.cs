using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Application.DTOs.Products;
using ShoppingApp.Application.Interfaces;

namespace ShoppingApp.API.Controllers;

[ApiController]
[Route("api/admin/products")]
[Authorize(Roles = "Admin")]
public class AdminProductsController : ControllerBase
{
    private readonly IProductService _products;
    public AdminProductsController(IProductService products) => _products = products;

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductDto dto)
    {
        var result = await _products.CreateAsync(dto);
        return result.Success ? CreatedAtAction("GetById", "Products", new { id = result.Data!.Id }, result.Data)
            : BadRequest(new { error = result.Error });
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateProductDto dto)
    {
        var result = await _products.UpdateAsync(id, dto);
        return result.Success ? Ok(result.Data) : NotFound(new { error = result.Error });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _products.DeleteAsync(id);
        return result.Success ? NoContent() : NotFound(new { error = result.Error });
    }
}
