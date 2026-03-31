using FluentValidation;
using LiftOps_BackEnd.Application.Features.Admins.DTOs;
using static LiftOps_BackEnd.Application.Validators.ValidationConstants;

namespace LiftOps_BackEnd.Application.Validators;

public class RegisterAdminDtoValidator : AbstractValidator<RegisterAdminDto>
{
    public RegisterAdminDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(Max256).WithMessage("Name is required.");
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(Max256).WithMessage("A valid email is required.");
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(Max256).WithMessage("Phone number is required.");
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6)
            .MaximumLength(Max256)
            .WithMessage("Password must be at least 6 characters.");
        RuleFor(x => x.Roles)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x)
            .Must(x => x.CompanyId.HasValue || !string.IsNullOrWhiteSpace(x.CompanySlug) || !string.IsNullOrWhiteSpace(x.CompanyName))
            .WithMessage("Provide one of CompanyId, CompanySlug, or CompanyName.");
    }
}
