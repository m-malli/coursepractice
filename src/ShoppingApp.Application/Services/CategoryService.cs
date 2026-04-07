using ShoppingApp.Application.Common;
using ShoppingApp.Application.DTOs.Categories;
using ShoppingApp.Application.Interfaces;
using ShoppingApp.Application.Mappings;
using ShoppingApp.Domain.Entities;
using ShoppingApp.Domain.Interfaces;

namespace ShoppingApp.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _uow;
    public CategoryService(IUnitOfWork uow) => _uow = uow;

    public async Task<ServiceResult<IEnumerable<CategoryDto>>> GetAllAsync()
    {
        var categories = await _uow.Categories.GetAllAsync();
        return ServiceResult<IEnumerable<CategoryDto>>.Ok(categories.Select(c => c.ToDto()));
    }

    public async Task<ServiceResult<CategoryDto>> CreateAsync(CreateCategoryDto dto)
    {
        var category = new Category
        {
            Name = dto.Name,
            Description = dto.Description,
            Slug = dto.Name.ToLowerInvariant().Replace(" ", "-")
        };
        await _uow.Categories.AddAsync(category);
        await _uow.SaveChangesAsync();
        return ServiceResult<CategoryDto>.Ok(category.ToDto());
    }
}
