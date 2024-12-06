using ECommerce.Users.Core.DTOs;
using FluentValidation;

namespace ECommerce.Users.Core.Validation;

public class LoginRequestValidation : AbstractValidator<LoginRequest> {
    public LoginRequestValidation()
    {
        RuleFor(x=>x.Email)
        .NotEmpty().WithMessage("Email is Required")
        .EmailAddress().WithMessage("Email is not valid");

        RuleFor(x=>x.Password)
        .NotEmpty().WithMessage("Password is Required")
        .MinimumLength(6).WithMessage("Password must be at least 6 characters long");
    }
};