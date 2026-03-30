using FluentValidation;
using LiftOps_BackEnd.Application.DTOs.Faults;
using static LiftOps_BackEnd.Application.Validators.ValidationConstants;

namespace LiftOps_BackEnd.Application.Validators;

public class CreateFaultTicketDtoValidator : AbstractValidator<CreateFaultTicketDto>
{
    public CreateFaultTicketDtoValidator()
    {
        RuleFor(x => x.CustomerName).NotEmpty().MaximumLength(Max256);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(Max256);
        RuleFor(x => x.ProjectAddress).NotEmpty().MaximumLength(Max256);
        RuleFor(x => x.ProjectNumber).MaximumLength(Max256);
        RuleFor(x => x.GoogleMapsLink).MaximumLength(Max500);

        RuleFor(x => x.ElevatorType).NotEmpty().MaximumLength(Max256);
        RuleFor(x => x.FaultDescription).NotEmpty().MaximumLength(Max2000);

        RuleFor(x => x.Severity).IsInEnum();
    }
}

public class AssignFaultTechnicianDtoValidator : AbstractValidator<AssignFaultTechnicianDto>
{
    public AssignFaultTechnicianDtoValidator()
    {
        RuleFor(x => x.TechnicianId).NotEmpty();
    }
}

public class ResolveFaultDtoValidator : AbstractValidator<ResolveFaultDto>
{
    public ResolveFaultDtoValidator()
    {
        RuleFor(x => x.Notes).NotEmpty().MaximumLength(Max2000);
    }
}

