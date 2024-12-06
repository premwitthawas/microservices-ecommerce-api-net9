using Ecommerce.OrderService.BusinessLogicLayer.DTOs;
using FluentValidation;

namespace Ecommerce.OrderService.BusinessLogicLayer.Validators;

public class OrderItemAddRequestValidator : AbstractValidator<OrderItemAddRequest>
{
    public OrderItemAddRequestValidator()
    {
        RuleFor(x => x.ProductID).NotEmpty()
        .WithErrorCode("ProductID is required");
        RuleFor(x => x.UnitPrice).NotEmpty()
        .GreaterThan(0)
        .WithErrorCode("UnitPrice is required and must be greater than 0");
        RuleFor(x => x.Quantity).NotEmpty().GreaterThan(0)
        .WithErrorCode("Quantity is required and must be greater than 0");
    }
}