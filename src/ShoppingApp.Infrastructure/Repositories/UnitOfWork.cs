using ShoppingApp.Domain.Interfaces;
using ShoppingApp.Infrastructure.Data;

namespace ShoppingApp.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;

    public UnitOfWork(AppDbContext db)
    {
        _db = db;
        Products = new ProductRepository(db);
        Categories = new CategoryRepository(db);
        Cart = new CartRepository(db);
        Orders = new OrderRepository(db);
        Coupons = new CouponRepository(db);
    }

    public IProductRepository Products { get; }
    public ICategoryRepository Categories { get; }
    public ICartRepository Cart { get; }
    public IOrderRepository Orders { get; }
    public ICouponRepository Coupons { get; }

    public Task<int> SaveChangesAsync() => _db.SaveChangesAsync();
    public void Dispose() => _db.Dispose();
}
