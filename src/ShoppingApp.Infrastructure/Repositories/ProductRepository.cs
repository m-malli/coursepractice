using Microsoft.EntityFrameworkCore;
using ShoppingApp.Domain.Entities;
using ShoppingApp.Domain.Interfaces;
using ShoppingApp.Infrastructure.Data;

namespace ShoppingApp.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _db;
    public ProductRepository(AppDbContext db) => _db = db;

    public async Task<Product?> GetByIdAsync(Guid id) =>
        await _db.Products.Include(p => p.Category).Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<(IEnumerable<Product> Items, int TotalCount)> GetAllAsync(
        string? search, Guid? categoryId, int page, int pageSize)
    {
        var query = _db.Products.Include(p => p.Category).Include(p => p.Images).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.Name.Contains(search) || (p.Description != null && p.Description.Contains(search)));
        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        var total = await query.CountAsync();
        var items = await query.OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }

    public async Task<Product> AddAsync(Product product)
    {
        await _db.Products.AddAsync(product);
        return product;
    }

    public Task UpdateAsync(Product product)
    {
        _db.Products.Update(product);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is not null) _db.Products.Remove(product);
    }
}
