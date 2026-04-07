using ShoppingApp.Domain.Entities;

namespace ShoppingApp.Domain.Interfaces;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(Guid id);
    Task<Category> AddAsync(Category category);
    Task UpdateAsync(Category category);
}
