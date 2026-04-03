using FluentValidation;
using LiftOps_BackEnd.Application.Features.PlatformAdmin.Commands;

namespace LiftOps_BackEnd.Application.Features.PlatformAdmin.Validators;

public class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
{
    public CreateCompanyCommandValidator()
    {
        RuleFor(x => x.Request.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Request.ContactEmail)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(x => x.Request.PlanId)
            .NotEmpty();
    }
}
