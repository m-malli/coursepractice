using Microsoft.EntityFrameworkCore;
using ShoppingApp.Domain.Entities;
using ShoppingApp.Domain.Interfaces;
using ShoppingApp.Infrastructure.Data;

namespace ShoppingApp.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _db;
    public CategoryRepository(AppDbContext db) => _db = db;

    public async Task<IEnumerable<Category>> GetAllAsync() =>
        await _db.Categories.Include(c => c.Products).ToListAsync();

    public async Task<Category?> GetByIdAsync(Guid id) =>
        await _db.Categories.FindAsync(id);

    public async Task<Category> AddAsync(Category category)
    {
        await _db.Categories.AddAsync(category);
        return category;
    }

    public Task UpdateAsync(Category category)
    {
        _db.Categories.Update(category);
        return Task.CompletedTask;
    }
}
