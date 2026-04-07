using ShoppingApp.Domain.Entities;

namespace ShoppingApp.Domain.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id);
    Task<(IEnumerable<Product> Items, int TotalCount)> GetAllAsync(string? search, Guid? categoryId, int page, int pageSize);
    Task<Product> AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(Guid id);
}
