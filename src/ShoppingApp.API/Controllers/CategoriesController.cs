using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Application.Interfaces;

namespace ShoppingApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categories;
    public CategoriesController(ICategoryService categories) => _categories = categories;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _categories.GetAllAsync();
        return Ok(result.Data);
    }
}
