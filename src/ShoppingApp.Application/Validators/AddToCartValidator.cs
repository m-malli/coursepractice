using FluentValidation;
using ShoppingApp.Application.DTOs.Cart;

namespace ShoppingApp.Application.Validators;

public class AddToCartValidator : AbstractValidator<AddToCartDto>
{
    public AddToCartValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0).LessThanOrEqualTo(100);
    }
}
