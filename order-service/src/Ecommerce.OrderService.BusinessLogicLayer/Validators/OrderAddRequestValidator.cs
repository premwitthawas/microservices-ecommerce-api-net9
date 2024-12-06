using Ecommerce.OrderService.BusinessLogicLayer.DTOs;
using FluentValidation;

namespace Ecommerce.OrderService.BusinessLogicLayer.Validators;

public class OrderAddRequestValidator : AbstractValidator<OrderAddRequest>
{
    public OrderAddRequestValidator()
    {
        RuleFor(x => x.UserID).NotEmpty()
        .WithErrorCode("UserId is required");
        RuleFor(x => x.OrderDate).NotEmpty()
        .WithErrorCode("OrderDate is required");
        RuleFor(x => x.OrderItems).NotEmpty()
        .WithErrorCode("OrderItems is required");
    }
}