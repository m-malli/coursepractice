using Moq;
using ShoppingApp.Application.Common;
using ShoppingApp.Application.DTOs.Orders;
using ShoppingApp.Application.Interfaces;
using ShoppingApp.Application.Services;
using ShoppingApp.Domain.Entities;
using ShoppingApp.Domain.Interfaces;

namespace ShoppingApp.Tests.Application;

public class OrderServiceTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<IPaymentService> _payment = new();

    private OrderService CreateService() => new(_uow.Object, _payment.Object);

    [Fact]
    public async Task CreateFromCartAsync_EmptyCart_ReturnsFail()
    {
        var userId = Guid.NewGuid();
        _uow.Setup(u => u.Cart.GetByUserIdAsync(userId)).ReturnsAsync(new List<CartItem>());

        var svc = CreateService();
        var result = await svc.CreateFromCartAsync(userId, new CreateOrderDto("123 Street", null));

        Assert.False(result.Success);
        Assert.Equal("Cart is empty.", result.Error);
    }

    [Fact]
    public async Task CreateFromCartAsync_PaymentFails_ReturnsFail()
    {
        var userId = Guid.NewGuid();
        var product = new Product { Id = Guid.NewGuid(), Name = "Item", Price = 50, StockQuantity = 10 };
        var cartItems = new List<CartItem>
        {
            new() { ProductId = product.Id, Quantity = 2, Product = product }
        };

        _uow.Setup(u => u.Cart.GetByUserIdAsync(userId)).ReturnsAsync(cartItems);
        _uow.Setup(u => u.Products.GetByIdAsync(product.Id)).ReturnsAsync(product);
        _payment.Setup(p => p.ProcessPaymentAsync(It.IsAny<decimal>(), It.IsAny<string>()))
            .ReturnsAsync(ServiceResult<string>.Fail("Card declined"));

        var svc = CreateService();
        var result = await svc.CreateFromCartAsync(userId, new CreateOrderDto("123 Street", null));

        Assert.False(result.Success);
        Assert.Contains("Payment failed", result.Error);
    }

    [Fact]
    public async Task CreateFromCartAsync_ValidCart_CreatesOrderAndClearsCart()
    {
        var userId = Guid.NewGuid();
        var product = new Product { Id = Guid.NewGuid(), Name = "Widget", Price = 25, StockQuantity = 10 };
        var cartItems = new List<CartItem>
        {
            new() { ProductId = product.Id, Quantity = 2, Product = product }
        };

        _uow.Setup(u => u.Cart.GetByUserIdAsync(userId)).ReturnsAsync(cartItems);
        _uow.Setup(u => u.Products.GetByIdAsync(product.Id)).ReturnsAsync(product);
        _uow.Setup(u => u.Orders.AddAsync(It.IsAny<Order>())).ReturnsAsync((Order o) => o);
        _uow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        _payment.Setup(p => p.ProcessPaymentAsync(It.IsAny<decimal>(), It.IsAny<string>()))
            .ReturnsAsync(ServiceResult<string>.Ok("TXN-123"));

        var svc = CreateService();
        var result = await svc.CreateFromCartAsync(userId, new CreateOrderDto("123 Main St", null));

        Assert.True(result.Success);
        Assert.Equal(50m, result.Data!.TotalAmount); // 25 * 2
        Assert.Equal("Confirmed", result.Data.Status);
        _uow.Verify(u => u.Cart.ClearAsync(userId), Times.Once);
    }

    [Fact]
    public async Task CreateFromCartAsync_WithValidCoupon_AppliesDiscount()
    {
        var userId = Guid.NewGuid();
        var product = new Product { Id = Guid.NewGuid(), Name = "Item", Price = 100, StockQuantity = 5 };
        var cartItems = new List<CartItem>
        {
            new() { ProductId = product.Id, Quantity = 1, Product = product }
        };
        var coupon = new Coupon
        {
            Code = "SAVE10", DiscountPercent = 10, MaxDiscountAmount = null,
            MinOrderAmount = 0, UsageLimit = 10, TimesUsed = 0,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };

        _uow.Setup(u => u.Cart.GetByUserIdAsync(userId)).ReturnsAsync(cartItems);
        _uow.Setup(u => u.Products.GetByIdAsync(product.Id)).ReturnsAsync(product);
        _uow.Setup(u => u.Coupons.GetByCodeAsync("SAVE10")).ReturnsAsync(coupon);
        _uow.Setup(u => u.Orders.AddAsync(It.IsAny<Order>())).ReturnsAsync((Order o) => o);
        _uow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        _payment.Setup(p => p.ProcessPaymentAsync(It.IsAny<decimal>(), It.IsAny<string>()))
            .ReturnsAsync(ServiceResult<string>.Ok("TXN-456"));

        var svc = CreateService();
        var result = await svc.CreateFromCartAsync(userId, new CreateOrderDto("456 Ave", "SAVE10"));

        Assert.True(result.Success);
        Assert.Equal(100m, result.Data!.SubTotal);
        Assert.Equal(10m, result.Data.DiscountAmount);
        Assert.Equal(90m, result.Data.TotalAmount);
        Assert.Equal(1, coupon.TimesUsed); // coupon usage incremented
    }

    [Fact]
    public async Task CreateFromCartAsync_InsufficientStock_ReturnsFail()
    {
        var userId = Guid.NewGuid();
        var product = new Product { Id = Guid.NewGuid(), Name = "Rare", Price = 50, StockQuantity = 1 };
        var cartItems = new List<CartItem>
        {
            new() { ProductId = product.Id, Quantity = 5, Product = product }
        };

        _uow.Setup(u => u.Cart.GetByUserIdAsync(userId)).ReturnsAsync(cartItems);
        _uow.Setup(u => u.Products.GetByIdAsync(product.Id)).ReturnsAsync(product);

        var svc = CreateService();
        var result = await svc.CreateFromCartAsync(userId, new CreateOrderDto("789 Blvd", null));

        Assert.False(result.Success);
        Assert.Contains("Insufficient stock", result.Error);
    }

    [Fact]
    public async Task GetByIdAsync_WrongUser_ReturnsFail()
    {
        var order = new Order { Id = Guid.NewGuid(), UserId = Guid.NewGuid() };
        _uow.Setup(u => u.Orders.GetByIdAsync(order.Id)).ReturnsAsync(order);

        var svc = CreateService();
        var result = await svc.GetByIdAsync(order.Id, Guid.NewGuid()); // different user

        Assert.False(result.Success);
        Assert.Equal("Order not found.", result.Error);
    }

    [Fact]
    public async Task CancelOrderAsync_ShippedOrder_ReturnsFail()
    {
        var userId = Guid.NewGuid();
        var order = new Order { Id = Guid.NewGuid(), UserId = userId, Status = OrderStatus.Shipped };
        _uow.Setup(u => u.Orders.GetByIdAsync(order.Id)).ReturnsAsync(order);

        var svc = CreateService();
        var result = await svc.CancelOrderAsync(order.Id, userId);

        Assert.False(result.Success);
        Assert.Contains("Cannot cancel", result.Error);
    }

    [Fact]
    public async Task UpdateStatusAsync_InvalidStatus_ReturnsFail()
    {
        var order = new Order { Id = Guid.NewGuid(), Status = OrderStatus.Pending };
        _uow.Setup(u => u.Orders.GetByIdAsync(order.Id)).ReturnsAsync(order);

        var svc = CreateService();
        var result = await svc.UpdateStatusAsync(order.Id, "NotAValidStatus");

        Assert.False(result.Success);
        Assert.Equal("Invalid status.", result.Error);
    }

    [Fact]
    public async Task UpdateStatusAsync_ValidStatus_UpdatesOrder()
    {
        var order = new Order { Id = Guid.NewGuid(), Status = OrderStatus.Pending };
        _uow.Setup(u => u.Orders.GetByIdAsync(order.Id)).ReturnsAsync(order);
        _uow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var svc = CreateService();
        var result = await svc.UpdateStatusAsync(order.Id, "Shipped");

        Assert.True(result.Success);
        Assert.Equal("Shipped", result.Data!.Status);
    }

    [Fact]
    public async Task UpdateStatusAsync_NotFound_ReturnsFail()
    {
        _uow.Setup(u => u.Orders.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Order?)null);

        var svc = CreateService();
        var result = await svc.UpdateStatusAsync(Guid.NewGuid(), "Shipped");

        Assert.False(result.Success);
        Assert.Equal("Order not found.", result.Error);
    }
}
