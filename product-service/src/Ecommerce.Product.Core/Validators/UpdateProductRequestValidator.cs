using Ecommerce.Product.Core.DTOs;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Product.Core.Validators;


public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(x => x.ProductName).NotEmpty().WithMessage("Product name is required");
        RuleFor(x => x.Category).IsInEnum().WithMessage("Product Category is required");
        RuleFor(x => x.UnitPrice).InclusiveBetween(0, double.MaxValue).WithMessage($"Unit price is Value between 0 and {double.MaxValue}");
        RuleFor(x => x.Quantity).InclusiveBetween(0, int.MaxValue).WithMessage($"Quantity in stock is Value between 0 and {int.MaxValue}");
    }
};