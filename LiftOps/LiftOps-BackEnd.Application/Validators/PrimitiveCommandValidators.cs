using FluentValidation;
using LiftOps_BackEnd.Application.Features.Admins.Commands.AssignRoles;
using LiftOps_BackEnd.Application.Features.Admins.Commands.RefreshToken;
using LiftOps_BackEnd.Application.Features.Maintenance.Commands.FreezeContract;
using LiftOps_BackEnd.Application.Features.Maintenance.Commands.FreezeElevator;
using LiftOps_BackEnd.Application.Features.Maintenance.Commands.StopContract;
using LiftOps_BackEnd.Application.Features.Maintenance.Commands.StopElevator;
using LiftOps_BackEnd.Application.Features.Emergency.Commands;
using static LiftOps_BackEnd.Application.Validators.ValidationConstants;

namespace LiftOps_BackEnd.Application.Validators;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.Token).NotEmpty().MaximumLength(Max2000);
        RuleFor(x => x.RefreshToken).NotEmpty().MaximumLength(Max2000);
    }
}

public class AssignRolesCommandValidator : AbstractValidator<AssignRolesCommand>
{
    public AssignRolesCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Roles).NotNull().NotEmpty();
        RuleForEach(x => x.Roles).NotEmpty().MaximumLength(Max256);
    }
}

public class ResolveEmergencyTicketCommandValidator : AbstractValidator<ResolveEmergencyTicketCommand>
{
    public ResolveEmergencyTicketCommandValidator()
    {
        RuleFor(x => x.TicketId).NotEmpty();
        When(x => x.Notes is not null, () => RuleFor(x => x.Notes!).MaximumLength(Max2000));
        When(x => x.Notes is null, () => RuleFor(x => x.Notes).Null());
    }
}

public class FreezeElevatorCommandValidator : AbstractValidator<FreezeElevatorCommand>
{
    public FreezeElevatorCommandValidator()
    {
        RuleFor(x => x.ElevatorId).NotEmpty();
        When(x => x.Reason is not null, () => RuleFor(x => x.Reason!).MaximumLength(Max2000));
    }
}

public class StopElevatorCommandValidator : AbstractValidator<StopElevatorCommand>
{
    public StopElevatorCommandValidator()
    {
        RuleFor(x => x.ElevatorId).NotEmpty();
        When(x => x.Reason is not null, () => RuleFor(x => x.Reason!).MaximumLength(Max2000));
    }
}

public class FreezeContractCommandValidator : AbstractValidator<FreezeContractCommand>
{
    public FreezeContractCommandValidator()
    {
        RuleFor(x => x.ContractId).NotEmpty();
        When(x => x.Reason is not null, () => RuleFor(x => x.Reason!).MaximumLength(Max2000));
    }
}

public class StopContractCommandValidator : AbstractValidator<StopContractCommand>
{
    public StopContractCommandValidator()
    {
        RuleFor(x => x.ContractId).NotEmpty();
        When(x => x.Reason is not null, () => RuleFor(x => x.Reason!).MaximumLength(Max2000));
    }
}

