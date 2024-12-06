using Ecommerce.OrderService.BusinessLogicLayer.DTOs;
using FluentValidation;

namespace Ecommerce.OrderService.BusinessLogicLayer.Validators;

public class OrderUpdateRequestValidator : AbstractValidator<OrderUpdateRequest>
{
    public OrderUpdateRequestValidator()
    {
        RuleFor(x => x.OrderID).NotEmpty()
        .WithErrorCode("OrderID cannot be empty");
        RuleFor(x => x.UserID).NotEmpty().WithErrorCode("UserID cannot be empty");
        RuleFor(x => x.OrderDate).NotEmpty().WithErrorCode("OrderDate cannot be empty");
        RuleFor(x => x.OrderItems).NotEmpty().WithErrorCode("OrderItems cannot be empty");
    }
}