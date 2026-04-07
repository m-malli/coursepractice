using ShoppingApp.Application.Common;
using ShoppingApp.Application.DTOs.Products;
using ShoppingApp.Application.Interfaces;
using ShoppingApp.Application.Mappings;
using ShoppingApp.Domain.Entities;
using ShoppingApp.Domain.Interfaces;

namespace ShoppingApp.Application.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _uow;
    public ProductService(IUnitOfWork uow) => _uow = uow;

    public async Task<ServiceResult<ProductDto>> GetByIdAsync(Guid id)
    {
        var product = await _uow.Products.GetByIdAsync(id);
        return product is null
            ? ServiceResult<ProductDto>.Fail("Product not found.")
            : ServiceResult<ProductDto>.Ok(product.ToDto());
    }

    public async Task<ServiceResult<PagedResult<ProductDto>>> GetAllAsync(
        string? search, Guid? categoryId, int page, int pageSize)
    {
        var (items, total) = await _uow.Products.GetAllAsync(search, categoryId, page, pageSize);
        var result = new PagedResult<ProductDto>
        {
            Items = items.Select(p => p.ToDto()),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
        return ServiceResult<PagedResult<ProductDto>>.Ok(result);
    }

    public async Task<ServiceResult<ProductDto>> CreateAsync(CreateProductDto dto)
    {
        var product = new Product
        {
            Name = dto.Name, Description = dto.Description, Price = dto.Price,
            DiscountPrice = dto.DiscountPrice, SKU = dto.SKU,
            StockQuantity = dto.StockQuantity, CategoryId = dto.CategoryId
        };
        await _uow.Products.AddAsync(product);
        await _uow.SaveChangesAsync();
        return ServiceResult<ProductDto>.Ok(product.ToDto());
    }

    public async Task<ServiceResult<ProductDto>> UpdateAsync(Guid id, UpdateProductDto dto)
    {
        var product = await _uow.Products.GetByIdAsync(id);
        if (product is null) return ServiceResult<ProductDto>.Fail("Product not found.");

        if (dto.Name is not null) product.Name = dto.Name;
        if (dto.Description is not null) product.Description = dto.Description;
        if (dto.Price.HasValue) product.Price = dto.Price.Value;
        if (dto.DiscountPrice.HasValue) product.DiscountPrice = dto.DiscountPrice;
        if (dto.SKU is not null) product.SKU = dto.SKU;
        if (dto.StockQuantity.HasValue) product.StockQuantity = dto.StockQuantity.Value;
        if (dto.IsActive.HasValue) product.IsActive = dto.IsActive.Value;
        if (dto.CategoryId.HasValue) product.CategoryId = dto.CategoryId.Value;
        product.UpdatedAt = DateTime.UtcNow;

        await _uow.Products.UpdateAsync(product);
        await _uow.SaveChangesAsync();
        return ServiceResult<ProductDto>.Ok(product.ToDto());
    }

    public async Task<ServiceResult<bool>> DeleteAsync(Guid id)
    {
        await _uow.Products.DeleteAsync(id);
        await _uow.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true);
    }
}
