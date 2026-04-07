using ShoppingApp.Application.Common;
using ShoppingApp.Application.DTOs.Categories;

namespace ShoppingApp.Application.Interfaces;

public interface ICategoryService
{
    Task<ServiceResult<IEnumerable<CategoryDto>>> GetAllAsync();
    Task<ServiceResult<CategoryDto>> CreateAsync(CreateCategoryDto dto);
}
