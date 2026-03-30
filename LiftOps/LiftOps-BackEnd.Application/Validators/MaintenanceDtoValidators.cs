using System;
using System.Linq;
using FluentValidation;
using LiftOps_BackEnd.Application.DTOs.Maintenance;
using LiftOps_BackEnd.Application.DTOs;
using static LiftOps_BackEnd.Application.Validators.ValidationConstants;

namespace LiftOps_BackEnd.Application.Validators;

// Note: these validators focus on the DTO shapes received from write endpoints
// (FromBody actions) to prevent invalid / overlong data from reaching handlers.

public class CreateMaintenanceContractDtoValidator : AbstractValidator<CreateMaintenanceContractDto>
{
    public CreateMaintenanceContractDtoValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.EndDate).NotEmpty();
        RuleFor(x => x.PricePerMonth).GreaterThan(0);
        RuleFor(x => x.FreeMonths).GreaterThanOrEqualTo(0);

        When(x => x.TechnicianId.HasValue, () => RuleFor(x => x.TechnicianId!.Value).NotEmpty());
    }
}

public class MaintenanceContractDtoValidator : AbstractValidator<MaintenanceContractDto>
{
    public MaintenanceContractDtoValidator()
    {
        RuleFor(x => x.ProjectNumber).NotEmpty().MaximumLength(Max256);
        When(x => x.ProjectAddress is not null, () => RuleFor(x => x.ProjectAddress!).MaximumLength(Max256));
        When(x => x.City is not null, () => RuleFor(x => x.City).NotEmpty().MaximumLength(Max256));
        When(x => x.GoogleMapsLink is not null, () => RuleFor(x => x.GoogleMapsLink!).MaximumLength(Max500));

        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.EndDate).NotEmpty();
        RuleFor(x => x.PricePerMonth).GreaterThan(0);
        RuleFor(x => x.FreeMonths).GreaterThanOrEqualTo(0);

        When(x => x.Notes is not null, () => RuleFor(x => x.Notes!).MaximumLength(Max2000));
    }
}

public class CreateMaintenanceElevatorDtoValidator : AbstractValidator<CreateMaintenanceElevatorDto>
{
    public CreateMaintenanceElevatorDtoValidator()
    {
        RuleFor(x => x.Type).NotEmpty().MaximumLength(Max256);
        RuleFor(x => x.NumberOfStops).GreaterThan(0);
        RuleFor(x => x.NumberOfFloors).GreaterThan(0);
        When(x => x.Notes is not null, () => RuleFor(x => x.Notes!).MaximumLength(Max2000));
    }
}

public class CreateMaintenanceProjectDtoValidator : AbstractValidator<CreateMaintenanceProjectDto>
{
    public CreateMaintenanceProjectDtoValidator()
    {
        RuleFor(x => x.Customer).NotNull();
        RuleFor(x => x.Contract).NotNull().SetValidator(new MaintenanceContractDtoValidator());

        RuleFor(x => x.Elevators).NotNull().NotEmpty();
        RuleForEach(x => x.Elevators).SetValidator(new CreateMaintenanceElevatorDtoValidator());
    }
}

public class ScheduleVisitDtoValidator : AbstractValidator<ScheduleVisitDto>
{
    public ScheduleVisitDtoValidator()
    {
        RuleFor(x => x.ElevatorId).NotEmpty();
        RuleFor(x => x.VisitDate).NotEmpty();
        When(x => x.TechnicianId.HasValue, () => RuleFor(x => x.TechnicianId!.Value).NotEmpty());
    }
}

public class PartUsageDtoValidator : AbstractValidator<PartUsageDto>
{
    public PartUsageDtoValidator()
    {
        RuleFor(x => x.ItemId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}

public class VisitChecklistItemDtoValidator : AbstractValidator<VisitChecklistItemDto>
{
    public VisitChecklistItemDtoValidator()
    {
        RuleFor(x => x.ChecklistItemId).NotEmpty();
        When(x => x.Notes is not null, () => RuleFor(x => x.Notes!).MaximumLength(Max2000));
    }
}

public class CompleteVisitDtoValidator : AbstractValidator<CompleteVisitDto>
{
    public CompleteVisitDtoValidator()
    {
        RuleFor(x => x.VisitId).NotEmpty();
        RuleFor(x => x.Notes).NotEmpty().MaximumLength(Max2000);

        When(x => x.PaymentNotes is not null, () => RuleFor(x => x.PaymentNotes!).MaximumLength(Max2000));

        RuleFor(x => x.PartsUsed).NotNull();
        RuleForEach(x => x.PartsUsed).SetValidator(new PartUsageDtoValidator());

        RuleFor(x => x.ChecklistItems).NotNull();
        RuleForEach(x => x.ChecklistItems).SetValidator(new VisitChecklistItemDtoValidator());
    }
}

public class CreateMaintenanceChecklistItemDtoValidator : AbstractValidator<CreateMaintenanceChecklistItemDto>
{
    public CreateMaintenanceChecklistItemDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(Max256);
        When(x => x.Description is not null, () => RuleFor(x => x.Description!).MaximumLength(Max2000));
        RuleFor(x => x.Order).GreaterThanOrEqualTo(0);
    }
}

public class UpdateMaintenanceChecklistItemDtoValidator : AbstractValidator<UpdateMaintenanceChecklistItemDto>
{
    public UpdateMaintenanceChecklistItemDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(Max256);
        When(x => x.Description is not null, () => RuleFor(x => x.Description!).MaximumLength(Max2000));
        RuleFor(x => x.Order).GreaterThanOrEqualTo(0);
    }
}

public class UpdateMaintenanceContractDtoValidator : AbstractValidator<UpdateMaintenanceContractDto>
{
    public UpdateMaintenanceContractDtoValidator()
    {
        RuleFor(x => x.ProjectNumber).NotEmpty().MaximumLength(Max256);

        When(x => x.ProjectAddress is not null, () => RuleFor(x => x.ProjectAddress!).MaximumLength(Max256));
        When(x => x.City is not null, () => RuleFor(x => x.City!).MaximumLength(Max256));
        When(x => x.GoogleMapsLink is not null, () => RuleFor(x => x.GoogleMapsLink!).MaximumLength(Max500));

        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.EndDate).NotEmpty();
        RuleFor(x => x.PricePerMonth).GreaterThan(0);
        RuleFor(x => x.FreeMonths).GreaterThanOrEqualTo(0);

        When(x => x.TechnicianId.HasValue, () => RuleFor(x => x.TechnicianId!.Value).NotEmpty());
    }
}

public class UpdateMaintenanceElevatorDtoValidator : AbstractValidator<UpdateMaintenanceElevatorDto>
{
    public UpdateMaintenanceElevatorDtoValidator()
    {
        RuleFor(x => x.Type).NotEmpty().MaximumLength(Max256);
        RuleFor(x => x.NumberOfStops).GreaterThan(0);
        RuleFor(x => x.NumberOfFloors).GreaterThan(0);
    }
}

public class FreezeElevatorDtoValidator : AbstractValidator<FreezeElevatorDto>
{
    public FreezeElevatorDtoValidator()
    {
        When(x => x.Reason is not null, () => RuleFor(x => x.Reason!).MaximumLength(Max2000));
    }
}

public class StopElevatorDtoValidator : AbstractValidator<StopElevatorDto>
{
    public StopElevatorDtoValidator()
    {
        When(x => x.Reason is not null, () => RuleFor(x => x.Reason!).MaximumLength(Max2000));
    }
}

public class FreezeContractDtoValidator : AbstractValidator<FreezeContractDto>
{
    public FreezeContractDtoValidator()
    {
        When(x => x.Reason is not null, () => RuleFor(x => x.Reason!).MaximumLength(Max2000));
    }
}

public class StopContractDtoValidator : AbstractValidator<StopContractDto>
{
    public StopContractDtoValidator()
    {
        When(x => x.Reason is not null, () => RuleFor(x => x.Reason!).MaximumLength(Max2000));
    }
}

public class AssignVisitToTechnicianDtoValidator : AbstractValidator<AssignVisitToTechnicianDto>
{
    public AssignVisitToTechnicianDtoValidator()
    {
        RuleFor(x => x.VisitId).NotEmpty();
        When(x => x.TechnicianId.HasValue, () => RuleFor(x => x.TechnicianId!.Value).NotEmpty());
        RuleFor(x => x.AssignmentDate).NotEmpty();
        When(x => x.Notes is not null, () => RuleFor(x => x.Notes!).MaximumLength(Max2000));
    }
}

public class UpdateVisitStatusDtoValidator : AbstractValidator<UpdateVisitStatusDto>
{
    public UpdateVisitStatusDtoValidator()
    {
        RuleFor(x => x.VisitId).NotEmpty();
        RuleFor(x => x.Status).NotEmpty().MaximumLength(Max256);
        RuleFor(x => x.Status)
            .Must(s => s == "InProgress" || s == "Done")
            .WithMessage("Status must be either 'InProgress' or 'Done'.");
    }
}

public class CancelVisitsByDateDtoValidator : AbstractValidator<CancelVisitsByDateDto>
{
    public CancelVisitsByDateDtoValidator()
    {
        RuleFor(x => x.Date).NotEmpty();
    }
}

public class UpdateVisitOrderDtoValidator : AbstractValidator<UpdateVisitOrderDto>
{
    public UpdateVisitOrderDtoValidator()
    {
        RuleFor(x => x.Date).NotEmpty();
        RuleFor(x => x.VisitIds).NotNull().NotEmpty();
        RuleForEach(x => x.VisitIds).Must(id => id != Guid.Empty);
    }
}

public class AssignTechniciansToContractVisitsDtoValidator : AbstractValidator<AssignTechniciansToContractVisitsDto>
{
    public AssignTechniciansToContractVisitsDtoValidator()
    {
        RuleFor(x => x.ContractId).NotEmpty();
        RuleFor(x => x.TechnicianIds).NotNull().NotEmpty();
        RuleForEach(x => x.TechnicianIds).Must(id => id != Guid.Empty);
        RuleFor(x => x.AssignmentDate).NotEmpty();
        When(x => x.Notes is not null, () => RuleFor(x => x.Notes!).MaximumLength(Max2000));
    }
}

