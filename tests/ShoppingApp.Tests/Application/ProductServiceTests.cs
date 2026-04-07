using Moq;
using ShoppingApp.Application.DTOs.Products;
using ShoppingApp.Application.Services;
using ShoppingApp.Domain.Entities;
using ShoppingApp.Domain.Interfaces;

namespace ShoppingApp.Tests.Application;

public class ProductServiceTests
{
    private readonly Mock<IUnitOfWork> _uow = new();

    [Fact]
    public async Task GetByIdAsync_ExistingProduct_ReturnsProduct()
    {
        var product = new Product
        {
            Id = Guid.NewGuid(), Name = "Test", Price = 9.99m,
            SKU = "TST-001", Category = new Category { Name = "Cat" }
        };
        _uow.Setup(u => u.Products.GetByIdAsync(product.Id)).ReturnsAsync(product);

        var svc = new ProductService(_uow.Object);
        var result = await svc.GetByIdAsync(product.Id);

        Assert.True(result.Success);
        Assert.Equal("Test", result.Data!.Name);
    }

    [Fact]
    public async Task GetByIdAsync_NotFound_ReturnsFail()
    {
        _uow.Setup(u => u.Products.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Product?)null);

        var svc = new ProductService(_uow.Object);
        var result = await svc.GetByIdAsync(Guid.NewGuid());

        Assert.False(result.Success);
        Assert.Equal("Product not found.", result.Error);
    }

    [Fact]
    public async Task CreateAsync_ValidProduct_ReturnsOk()
    {
        _uow.Setup(u => u.Products.AddAsync(It.IsAny<Product>()))
            .ReturnsAsync((Product p) => p);
        _uow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var svc = new ProductService(_uow.Object);
        var result = await svc.CreateAsync(new CreateProductDto(
            "Widget", "A widget", 19.99m, null, "WDG-001", 50, Guid.NewGuid()));

        Assert.True(result.Success);
        Assert.Equal("Widget", result.Data!.Name);
        Assert.Equal(19.99m, result.Data.Price);
    }

    [Fact]
    public async Task UpdateAsync_NotFound_ReturnsFail()
    {
        _uow.Setup(u => u.Products.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Product?)null);

        var svc = new ProductService(_uow.Object);
        var result = await svc.UpdateAsync(Guid.NewGuid(), new UpdateProductDto("New", null, null, null, null, null, null, null));

        Assert.False(result.Success);
        Assert.Equal("Product not found.", result.Error);
    }

    [Fact]
    public async Task UpdateAsync_PartialUpdate_OnlyChangesProvidedFields()
    {
        var product = new Product
        {
            Id = Guid.NewGuid(), Name = "Old", Price = 10m,
            SKU = "SKU-1", StockQuantity = 5, IsActive = true,
            Category = new Category { Name = "Cat" }
        };
        _uow.Setup(u => u.Products.GetByIdAsync(product.Id)).ReturnsAsync(product);
        _uow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var svc = new ProductService(_uow.Object);
        var result = await svc.UpdateAsync(product.Id, new UpdateProductDto("Updated", null, 20m, null, null, null, null, null));

        Assert.True(result.Success);
        Assert.Equal("Updated", result.Data!.Name);
        Assert.Equal(20m, result.Data.Price);
        Assert.Equal("SKU-1", result.Data.SKU); // unchanged
    }

    [Fact]
    public async Task GetAllAsync_ReturnsPagedResult()
    {
        var products = new List<Product>
        {
            new() { Id = Guid.NewGuid(), Name = "A", Price = 5, SKU = "A-1", Category = new Category { Name = "C1" } },
            new() { Id = Guid.NewGuid(), Name = "B", Price = 10, SKU = "B-1", Category = new Category { Name = "C1" } }
        };
        _uow.Setup(u => u.Products.GetAllAsync(null, null, 1, 20))
            .ReturnsAsync((products.AsEnumerable(), 2));

        var svc = new ProductService(_uow.Object);
        var result = await svc.GetAllAsync(null, null, 1, 20);

        Assert.True(result.Success);
        Assert.Equal(2, result.Data!.TotalCount);
        Assert.Equal(1, result.Data.TotalPages);
    }
}
