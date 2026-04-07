using Moq;
using ShoppingApp.Application.DTOs.Cart;
using ShoppingApp.Application.Services;
using ShoppingApp.Domain.Entities;
using ShoppingApp.Domain.Interfaces;

namespace ShoppingApp.Tests.Application;

public class CartServiceTests
{
    private readonly Mock<IUnitOfWork> _uow = new();

    [Fact]
    public async Task AddToCartAsync_ValidProduct_ReturnsCartItem()
    {
        var userId = Guid.NewGuid();
        var product = new Product { Id = Guid.NewGuid(), Name = "Item", Price = 10, StockQuantity = 5 };

        _uow.Setup(u => u.Products.GetByIdAsync(product.Id)).ReturnsAsync(product);
        _uow.Setup(u => u.Cart.GetItemAsync(userId, product.Id)).ReturnsAsync((CartItem?)null);
        _uow.Setup(u => u.Cart.AddAsync(It.IsAny<CartItem>())).ReturnsAsync((CartItem ci) => ci);
        _uow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var svc = new CartService(_uow.Object);
        var result = await svc.AddToCartAsync(userId, new AddToCartDto(product.Id, 2));

        Assert.True(result.Success);
        Assert.Equal(product.Id, result.Data!.ProductId);
    }

    [Fact]
    public async Task AddToCartAsync_InsufficientStock_Fails()
    {
        var product = new Product { Id = Guid.NewGuid(), StockQuantity = 1 };
        _uow.Setup(u => u.Products.GetByIdAsync(product.Id)).ReturnsAsync(product);

        var svc = new CartService(_uow.Object);
        var result = await svc.AddToCartAsync(Guid.NewGuid(), new AddToCartDto(product.Id, 5));

        Assert.False(result.Success);
        Assert.Contains("Insufficient", result.Error);
    }

    [Fact]
    public async Task AddToCartAsync_ProductNotFound_Fails()
    {
        _uow.Setup(u => u.Products.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Product?)null);

        var svc = new CartService(_uow.Object);
        var result = await svc.AddToCartAsync(Guid.NewGuid(), new AddToCartDto(Guid.NewGuid(), 1));

        Assert.False(result.Success);
        Assert.Equal("Product not found.", result.Error);
    }

    [Fact]
    public async Task AddToCartAsync_ExistingItem_MergesQuantity()
    {
        var userId = Guid.NewGuid();
        var product = new Product { Id = Guid.NewGuid(), Name = "Item", Price = 10, StockQuantity = 20 };
        var existingItem = new CartItem { UserId = userId, ProductId = product.Id, Quantity = 3, Product = product };

        _uow.Setup(u => u.Products.GetByIdAsync(product.Id)).ReturnsAsync(product);
        _uow.Setup(u => u.Cart.GetItemAsync(userId, product.Id)).ReturnsAsync(existingItem);
        _uow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var svc = new CartService(_uow.Object);
        var result = await svc.AddToCartAsync(userId, new AddToCartDto(product.Id, 2));

        Assert.True(result.Success);
        Assert.Equal(5, existingItem.Quantity); // 3 + 2
    }

    [Fact]
    public async Task UpdateQuantityAsync_ItemNotInCart_Fails()
    {
        _uow.Setup(u => u.Cart.GetItemAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync((CartItem?)null);

        var svc = new CartService(_uow.Object);
        var result = await svc.UpdateQuantityAsync(Guid.NewGuid(), Guid.NewGuid(), new UpdateCartItemDto(5));

        Assert.False(result.Success);
        Assert.Equal("Item not in cart.", result.Error);
    }

    [Fact]
    public async Task GetCartAsync_EmptyCart_ReturnsEmptyList()
    {
        var userId = Guid.NewGuid();
        _uow.Setup(u => u.Cart.GetByUserIdAsync(userId)).ReturnsAsync(new List<CartItem>());

        var svc = new CartService(_uow.Object);
        var result = await svc.GetCartAsync(userId);

        Assert.True(result.Success);
        Assert.Empty(result.Data!);
    }

    [Fact]
    public async Task ClearCartAsync_ReturnsSuccess()
    {
        var userId = Guid.NewGuid();
        _uow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var svc = new CartService(_uow.Object);
        var result = await svc.ClearCartAsync(userId);

        Assert.True(result.Success);
        _uow.Verify(u => u.Cart.ClearAsync(userId), Times.Once);
    }
}
