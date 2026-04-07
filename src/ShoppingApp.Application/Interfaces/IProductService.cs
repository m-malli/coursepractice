using ShoppingApp.Application.Common;
using ShoppingApp.Application.DTOs.Products;

namespace ShoppingApp.Application.Interfaces;

public interface IProductService
{
    Task<ServiceResult<ProductDto>> GetByIdAsync(Guid id);
    Task<ServiceResult<PagedResult<ProductDto>>> GetAllAsync(string? search, Guid? categoryId, int page, int pageSize);
    Task<ServiceResult<ProductDto>> CreateAsync(CreateProductDto dto);
    Task<ServiceResult<ProductDto>> UpdateAsync(Guid id, UpdateProductDto dto);
    Task<ServiceResult<bool>> DeleteAsync(Guid id);
}
