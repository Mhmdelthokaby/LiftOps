using FluentValidation;
using LiftOps_BackEnd.Application.Features.PlatformAdmin.Commands;

namespace LiftOps_BackEnd.Application.Features.PlatformAdmin.Validators;

public class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
{
    public CreateCompanyCommandValidator()
    {
        RuleFor(x => x.Request.CompanyName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Request.AdminEmail)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(x => x.Request.Password)
            .NotEmpty()
            .MinimumLength(8);
    }
}
