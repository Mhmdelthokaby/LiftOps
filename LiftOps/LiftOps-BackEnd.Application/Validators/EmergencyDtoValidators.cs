using FluentValidation;
using LiftOps_BackEnd.Application.DTOs.Emergency;
using static LiftOps_BackEnd.Application.Validators.ValidationConstants;

namespace LiftOps_BackEnd.Application.Validators;

public class CreateEmergencyTicketDtoValidator : AbstractValidator<CreateEmergencyTicketDto>
{
    public CreateEmergencyTicketDtoValidator()
    {
        RuleFor(x => x.Project).NotEmpty().MaximumLength(Max256);
        RuleFor(x => x.Location).NotEmpty().MaximumLength(Max256);
        RuleFor(x => x.UnitId).NotEmpty().MaximumLength(Max256);
        RuleFor(x => x.GoogleMapsLink).MaximumLength(Max500);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(Max2000);
        RuleFor(x => x.ReportedBy).NotEmpty().MaximumLength(Max256);
        RuleFor(x => x.Contact).NotEmpty().MaximumLength(Max256);

        RuleFor(x => x.Priority).IsInEnum();
    }
}

public class UpdateEmergencyTicketDtoValidator : AbstractValidator<UpdateEmergencyTicketDto>
{
    public UpdateEmergencyTicketDtoValidator()
    {
        RuleFor(x => x.Project).NotEmpty().MaximumLength(Max256);
        RuleFor(x => x.Location).NotEmpty().MaximumLength(Max256);
        RuleFor(x => x.UnitId).NotEmpty().MaximumLength(Max256);
        RuleFor(x => x.GoogleMapsLink).MaximumLength(Max500);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(Max2000);
        RuleFor(x => x.ReportedBy).NotEmpty().MaximumLength(Max256);
        RuleFor(x => x.Contact).NotEmpty().MaximumLength(Max256);

        RuleFor(x => x.Priority).IsInEnum();
        RuleFor(x => x.Status).IsInEnum();

        When(x => x.AssignedTechnicianId.HasValue, () => RuleFor(x => x.AssignedTechnicianId!.Value).NotEmpty());

        When(x => x.Notes is not null, () => RuleFor(x => x.Notes!).MaximumLength(Max2000));
    }
}

public class AssignEmergencyTechnicianDtoValidator : AbstractValidator<AssignEmergencyTechnicianDto>
{
    public AssignEmergencyTechnicianDtoValidator()
    {
        RuleFor(x => x.TechnicianId).NotEmpty();
    }
}

public class ResolveEmergencyTicketRequestValidator : AbstractValidator<ResolveEmergencyTicketRequest>
{
    public ResolveEmergencyTicketRequestValidator()
    {
        When(x => x.Notes is not null, () => RuleFor(x => x.Notes!).MaximumLength(Max2000));
    }
}

