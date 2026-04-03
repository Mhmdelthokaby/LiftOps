using FluentValidation;
using LiftOps_BackEnd.Application.Features.PlatformAdmin.Commands;

namespace LiftOps_BackEnd.Application.Features.PlatformAdmin.Validators;

public class CreatePlanCommandValidator : AbstractValidator<CreatePlanCommand>
{
    public CreatePlanCommandValidator()
    {
        RuleFor(x => x.Request.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Request.Code).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Request.MonthlyPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Request.YearlyPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Request.TrialDays).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Request.MaxUsers).GreaterThan(0);
        RuleFor(x => x.Request.MaxElevators).GreaterThan(0);
        RuleFor(x => x.Request.MaxMaintenanceContracts).GreaterThan(0);
        RuleFor(x => x.Request.MaxInstallationProjects).GreaterThan(0);
    }
}
