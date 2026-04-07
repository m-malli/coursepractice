using ShoppingApp.Application.DTOs.Auth;
using ShoppingApp.Application.DTOs.Cart;
using ShoppingApp.Application.DTOs.Products;
using ShoppingApp.Application.Validators;

namespace ShoppingApp.Tests.Application;

public class ValidatorTests
{
    // --- CreateProductValidator ---

    [Fact]
    public void CreateProductValidator_ValidInput_Passes()
    {
        var validator = new CreateProductValidator();
        var dto = new CreateProductDto("Widget", "Nice widget", 19.99m, null, "WDG-001", 10, Guid.NewGuid());
        var result = validator.Validate(dto);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void CreateProductValidator_EmptyName_Fails()
    {
        var validator = new CreateProductValidator();
        var dto = new CreateProductDto("", null, 19.99m, null, "SKU", 10, Guid.NewGuid());
        var result = validator.Validate(dto);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Fact]
    public void CreateProductValidator_ZeroPrice_Fails()
    {
        var validator = new CreateProductValidator();
        var dto = new CreateProductDto("Item", null, 0m, null, "SKU", 10, Guid.NewGuid());
        var result = validator.Validate(dto);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Price");
    }

    [Fact]
    public void CreateProductValidator_NegativePrice_Fails()
    {
        var validator = new CreateProductValidator();
        var dto = new CreateProductDto("Item", null, -5m, null, "SKU", 10, Guid.NewGuid());
        var result = validator.Validate(dto);
        Assert.False(result.IsValid);
    }

    [Fact]
    public void CreateProductValidator_NegativeStock_Fails()
    {
        var validator = new CreateProductValidator();
        var dto = new CreateProductDto("Item", null, 10m, null, "SKU", -1, Guid.NewGuid());
        var result = validator.Validate(dto);
        Assert.False(result.IsValid);
    }

    [Fact]
    public void CreateProductValidator_EmptySKU_Fails()
    {
        var validator = new CreateProductValidator();
        var dto = new CreateProductDto("Item", null, 10m, null, "", 10, Guid.NewGuid());
        var result = validator.Validate(dto);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "SKU");
    }

    [Fact]
    public void CreateProductValidator_EmptyCategoryId_Fails()
    {
        var validator = new CreateProductValidator();
        var dto = new CreateProductDto("Item", null, 10m, null, "SKU", 10, Guid.Empty);
        var result = validator.Validate(dto);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "CategoryId");
    }

    // --- RegisterValidator ---

    [Fact]
    public void RegisterValidator_ValidInput_Passes()
    {
        var validator = new RegisterValidator();
        var dto = new RegisterDto("user@example.com", "Password123", "John", "Doe");
        var result = validator.Validate(dto);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void RegisterValidator_InvalidEmail_Fails()
    {
        var validator = new RegisterValidator();
        var dto = new RegisterDto("not-an-email", "Password123", "John", "Doe");
        var result = validator.Validate(dto);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Email");
    }

    [Fact]
    public void RegisterValidator_ShortPassword_Fails()
    {
        var validator = new RegisterValidator();
        var dto = new RegisterDto("user@example.com", "short", "John", "Doe");
        var result = validator.Validate(dto);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Password");
    }

    [Fact]
    public void RegisterValidator_EmptyFirstName_Fails()
    {
        var validator = new RegisterValidator();
        var dto = new RegisterDto("user@example.com", "Password123", "", "Doe");
        var result = validator.Validate(dto);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "FirstName");
    }

    // --- AddToCartValidator ---

    [Fact]
    public void AddToCartValidator_ValidInput_Passes()
    {
        var validator = new AddToCartValidator();
        var dto = new AddToCartDto(Guid.NewGuid(), 1);
        var result = validator.Validate(dto);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void AddToCartValidator_ZeroQuantity_Fails()
    {
        var validator = new AddToCartValidator();
        var dto = new AddToCartDto(Guid.NewGuid(), 0);
        var result = validator.Validate(dto);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Quantity");
    }

    [Fact]
    public void AddToCartValidator_NegativeQuantity_Fails()
    {
        var validator = new AddToCartValidator();
        var dto = new AddToCartDto(Guid.NewGuid(), -1);
        var result = validator.Validate(dto);
        Assert.False(result.IsValid);
    }

    [Fact]
    public void AddToCartValidator_QuantityOver100_Fails()
    {
        var validator = new AddToCartValidator();
        var dto = new AddToCartDto(Guid.NewGuid(), 101);
        var result = validator.Validate(dto);
        Assert.False(result.IsValid);
    }

    [Fact]
    public void AddToCartValidator_EmptyProductId_Fails()
    {
        var validator = new AddToCartValidator();
        var dto = new AddToCartDto(Guid.Empty, 1);
        var result = validator.Validate(dto);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ProductId");
    }
}
