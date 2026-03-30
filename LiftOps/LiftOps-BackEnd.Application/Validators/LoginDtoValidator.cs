using FluentValidation;
using LiftOps_BackEnd.Application.Features.Admins.DTOs;
using static LiftOps_BackEnd.Application.Validators.ValidationConstants;

namespace LiftOps_BackEnd.Application.Validators;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(Max256)
            .WithMessage("A valid email is required.");
        RuleFor(x => x.Password).NotEmpty().MaximumLength(Max256).WithMessage("Password is required.");
    }
}
