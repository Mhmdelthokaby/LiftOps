using FluentValidation;
using LiftOps_BackEnd.Application.Features.Admins.DTOs;

namespace LiftOps_BackEnd.Application.Validators;

public class RegisterAdminDtoValidator : AbstractValidator<RegisterAdminDto>
{
    public RegisterAdminDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("A valid email is required.");
        RuleFor(x => x.Phone).NotEmpty().WithMessage("Phone number is required.");
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6).WithMessage("Password must be at least 6 characters.");
        RuleFor(x => x.Roles).NotNull();
    }
}
