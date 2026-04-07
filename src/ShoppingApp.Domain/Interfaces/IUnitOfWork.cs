namespace ShoppingApp.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IProductRepository Products { get; }
    ICategoryRepository Categories { get; }
    ICartRepository Cart { get; }
    IOrderRepository Orders { get; }
    ICouponRepository Coupons { get; }
    Task<int> SaveChangesAsync();
}
