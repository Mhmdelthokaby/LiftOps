using FluentValidation;
using LiftOps_BackEnd.Application.Features.PlatformAdmin.Commands;

namespace LiftOps_BackEnd.Application.Features.PlatformAdmin.Validators;

public class UpdateCompanyCommandValidator : AbstractValidator<UpdateCompanyCommand>
{
    public UpdateCompanyCommandValidator()
    {
        RuleFor(x => x.Request.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Request.PlanId)
            .Must(id => id is null || id != Guid.Empty)
            .WithMessage("Subscription plan id must be a valid Guid when provided.");

        RuleFor(x => x.Request.ContactPhone)
            .MaximumLength(50);
    }
}
