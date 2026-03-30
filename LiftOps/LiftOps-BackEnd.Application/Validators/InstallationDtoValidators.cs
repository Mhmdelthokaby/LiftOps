using System;
using FluentValidation;
using LiftOps_BackEnd.Application.DTOs.Installation;
using static LiftOps_BackEnd.Application.Validators.ValidationConstants;

namespace LiftOps_BackEnd.Application.Validators;

public class CustomerDtoValidator : AbstractValidator<CustomerDto>
{
    public CustomerDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(Max256);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(Max256);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(Max256);
        RuleFor(x => x.Address).NotEmpty().MaximumLength(Max256);
        RuleFor(x => x.City).NotEmpty().MaximumLength(Max256);
        RuleFor(x => x.ProjectNumber).NotEmpty().MaximumLength(Max256);
        RuleFor(x => x.GoogleMapsLink).MaximumLength(Max500);
    }
}

public class ContractDtoValidator : AbstractValidator<ContractDto>
{
    public ContractDtoValidator()
    {
        RuleFor(x => x.City).NotEmpty().MaximumLength(Max256);
        RuleFor(x => x.InstallationPricePerUnit).GreaterThan(0);
        RuleFor(x => x.TotalPrice).GreaterThan(0);
        RuleFor(x => x.ContractDate).NotEmpty();

        When(x => !string.IsNullOrWhiteSpace(x.ProjectAddress), () =>
        {
            RuleFor(x => x.ProjectAddress!).MaximumLength(Max256);
        });

        When(x => !string.IsNullOrWhiteSpace(x.GoogleMapsLink), () =>
        {
            RuleFor(x => x.GoogleMapsLink!).MaximumLength(Max500);
        });

        When(x => x.Notes is not null, () => RuleFor(x => x.Notes!).MaximumLength(Max2000));
    }
}

public class CreateElevatorDtoValidator : AbstractValidator<CreateElevatorDto>
{
    public CreateElevatorDtoValidator()
    {
        RuleFor(x => x.ElevatorType).NotEmpty().MaximumLength(Max256);
        RuleFor(x => x.FloorsCount).GreaterThan(0);
        RuleFor(x => x.StopsCount).GreaterThan(0);
        RuleFor(x => x.PitType).NotEmpty().MaximumLength(Max256);
        RuleFor(x => x.PitWidth).GreaterThan(0);
        RuleFor(x => x.PitDepth).GreaterThan(0);
        RuleFor(x => x.LastFloorHeight).GreaterThan(0);
        RuleFor(x => x.HoleDepth).GreaterThan(0);
        RuleFor(x => x.TravelLength).GreaterThan(0);
        RuleFor(x => x.Price).GreaterThan(0);

        When(x => x.Notes is not null, () => RuleFor(x => x.Notes!).MaximumLength(Max2000));
    }
}

public class CreateProjectDtoValidator : AbstractValidator<CreateProjectDto>
{
    public CreateProjectDtoValidator()
    {
        RuleFor(x => x.Customer).NotNull().SetValidator(new CustomerDtoValidator());
        RuleFor(x => x.Contract).NotNull().SetValidator(new ContractDtoValidator());
        RuleFor(x => x.Elevators).NotNull().NotEmpty();
        RuleForEach(x => x.Elevators).SetValidator(new CreateElevatorDtoValidator());
    }
}

public class AddElevatorDtoValidator : AbstractValidator<AddElevatorDto>
{
    public AddElevatorDtoValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.Elevator).NotNull().SetValidator(new CreateElevatorDtoValidator());
    }
}

public class StartStageDtoValidator : AbstractValidator<StartStageDto>
{
    public StartStageDtoValidator()
    {
        RuleFor(x => x.StageId).NotEmpty();
        // StartDate is optional; if provided keep it valid
        When(x => x.StartDate.HasValue, () =>
        {
            RuleFor(x => x.StartDate.Value).NotEqual(default(DateTime));
        });
    }
}

public class TechnicianRatingDtoValidator : AbstractValidator<TechnicianRatingDto>
{
    public TechnicianRatingDtoValidator()
    {
        RuleFor(x => x.TechnicianId).NotEmpty();
        RuleFor(x => x.Rating).InclusiveBetween(0, 5);
    }
}

public class CompleteStageDtoValidator : AbstractValidator<CompleteStageDto>
{
    public CompleteStageDtoValidator()
    {
        RuleFor(x => x.StageId).NotEmpty();

        When(x => x.SupplyCost.HasValue, () => RuleFor(x => x.SupplyCost!.Value).GreaterThanOrEqualTo(0));
        When(x => x.Price.HasValue, () => RuleFor(x => x.Price!.Value).GreaterThanOrEqualTo(0));

        When(x => x.Notes is not null, () => RuleFor(x => x.Notes!).MaximumLength(Max2000));

        When(x => x.TechnicianRatings is not null, () =>
        {
            RuleForEach(x => x.TechnicianRatings).SetValidator(new TechnicianRatingDtoValidator());
        });
    }
}

public class PartSelectionDtoValidator : AbstractValidator<PartSelectionDto>
{
    public PartSelectionDtoValidator()
    {
        RuleFor(x => x.InventoryItemId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}

public class AddStagePartsDtoValidator : AbstractValidator<AddStagePartsDto>
{
    public AddStagePartsDtoValidator()
    {
        RuleFor(x => x.StageId).NotEmpty();
        RuleFor(x => x.Parts).NotNull().NotEmpty();
        RuleForEach(x => x.Parts).SetValidator(new PartSelectionDtoValidator());
    }
}

public class UpdateStageDtoValidator : AbstractValidator<UpdateStageDto>
{
    public UpdateStageDtoValidator()
    {
        RuleFor(x => x.StageId).NotEmpty();

        When(x => x.Parts is not null, () =>
        {
            RuleFor(x => x.Parts!).NotEmpty();
            RuleForEach(x => x.Parts!).SetValidator(new PartSelectionDtoValidator());
        });

        When(x => x.Notes is not null, () => RuleFor(x => x.Notes!).MaximumLength(Max2000));

        When(x => x.SupplyCost.HasValue, () => RuleFor(x => x.SupplyCost!.Value).GreaterThanOrEqualTo(0));
        When(x => x.StagePrice.HasValue, () => RuleFor(x => x.StagePrice!.Value).GreaterThanOrEqualTo(0));

        When(x => x.TechnicianIds is not null, () =>
        {
            RuleFor(x => x.TechnicianIds!).NotEmpty();
            RuleForEach(x => x.TechnicianIds!).Must(id => id != Guid.Empty).WithMessage("TechnicianIds cannot contain empty Guid values.");
        });
    }
}

public class UpdateCustomerDtoValidator : AbstractValidator<UpdateCustomerDto>
{
    public UpdateCustomerDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(Max256);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(Max256);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(Max256);
        RuleFor(x => x.Address).NotEmpty().MaximumLength(Max256);
        RuleFor(x => x.City).NotEmpty().MaximumLength(Max256);
        RuleFor(x => x.ProjectNumber).NotEmpty().MaximumLength(Max256);
        RuleFor(x => x.GoogleMapsLink).MaximumLength(Max500);
    }
}

public class UpdateContractDtoValidator : AbstractValidator<UpdateContractDto>
{
    public UpdateContractDtoValidator()
    {
        When(x => !string.IsNullOrWhiteSpace(x.ProjectAddress), () =>
            RuleFor(x => x.ProjectAddress!).MaximumLength(Max256));

        RuleFor(x => x.City).NotEmpty().MaximumLength(Max256);

        RuleFor(x => x.InstallationPricePerUnit).GreaterThan(0);
        RuleFor(x => x.TotalPrice).GreaterThan(0);
        RuleFor(x => x.ContractDate).NotEmpty();

        When(x => !string.IsNullOrWhiteSpace(x.GoogleMapsLink), () =>
            RuleFor(x => x.GoogleMapsLink!).MaximumLength(Max500));

        When(x => x.Notes is not null, () => RuleFor(x => x.Notes!).MaximumLength(Max2000));
    }
}

public class UpdateProjectDtoValidator : AbstractValidator<UpdateProjectDto>
{
    public UpdateProjectDtoValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.Customer).NotNull().SetValidator(new UpdateCustomerDtoValidator());
        RuleFor(x => x.Contract).NotNull().SetValidator(new UpdateContractDtoValidator());
    }
}

public class AssignTechnicianDtoValidator : AbstractValidator<AssignTechnicianDto>
{
    public AssignTechnicianDtoValidator()
    {
        RuleFor(x => x.ElevatorId).NotEmpty();
        RuleFor(x => x.TechnicianIds).NotNull().NotEmpty();
        RuleForEach(x => x.TechnicianIds).Must(id => id != Guid.Empty);
    }
}

public class CreateInspectionRequestDtoValidator : AbstractValidator<CreateInspectionRequestDto>
{
    public CreateInspectionRequestDtoValidator()
    {
        When(x => x.ClientId.HasValue, () => RuleFor(x => x.ClientId!.Value).NotEmpty());

        RuleFor(x => x.ClientName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ClientPhone).NotEmpty().MaximumLength(20);
        RuleFor(x => x.ClientEmail).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.ProjectAddress).NotEmpty().MaximumLength(500);
        When(x => x.GoogleMapsLink is not null, () =>
        {
            RuleFor(x => x.GoogleMapsLink!).MaximumLength(500);
            RuleFor(x => x.GoogleMapsLink!).Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                .WithMessage("Invalid Google Maps link");
        });

        RuleFor(x => x.NumberOfElevatorsRequired).InclusiveBetween(1, 100);
        RuleFor(x => x.ElevatorType).NotEmpty().MaximumLength(100);
        When(x => x.Notes is not null, () => RuleFor(x => x.Notes!).MaximumLength(1000));
    }
}

public class UpdateInspectionTechnicalDataDtoValidator : AbstractValidator<UpdateInspectionTechnicalDataDto>
{
    public UpdateInspectionTechnicalDataDtoValidator()
    {
        When(x => x.ShaftType is not null, () => RuleFor(x => x.ShaftType!).MaximumLength(50));

        When(x => x.ShaftWidth.HasValue, () => RuleFor(x => x.ShaftWidth!.Value).InclusiveBetween(0, 999999.99m));
        When(x => x.ShaftDepth.HasValue, () => RuleFor(x => x.ShaftDepth!.Value).InclusiveBetween(0, 999999.99m));
        When(x => x.LastFloorHeight.HasValue, () => RuleFor(x => x.LastFloorHeight!.Value).InclusiveBetween(0, 999999.99m));
        When(x => x.PitDepth.HasValue, () => RuleFor(x => x.PitDepth!.Value).InclusiveBetween(0, 999999.99m));
        When(x => x.TravelHeight.HasValue, () => RuleFor(x => x.TravelHeight!.Value).InclusiveBetween(0, 999999.99m));

        When(x => x.TechnicalNotes is not null, () => RuleFor(x => x.TechnicalNotes!).MaximumLength(2000));
    }
}

public class CreateOfferDtoValidator : AbstractValidator<CreateOfferDto>
{
    public CreateOfferDtoValidator()
    {
        RuleFor(x => x.InspectionRequestId).NotEmpty();
        RuleFor(x => x.InstallationPricePerUnit).GreaterThan(0);
        RuleFor(x => x.TotalInstallationPrice).GreaterThan(0);
        RuleFor(x => x.EstimatedStartDate).NotEmpty();
        RuleFor(x => x.EstimatedEndDate).NotEmpty();
        When(x => x.Notes is not null, () => RuleFor(x => x.Notes!).MaximumLength(1000));
    }
}

public class UpdateOfferDtoValidator : AbstractValidator<UpdateOfferDto>
{
    public UpdateOfferDtoValidator()
    {
        When(x => x.InstallationPricePerUnit.HasValue, () => RuleFor(x => x.InstallationPricePerUnit!.Value).GreaterThan(0));
        When(x => x.TotalInstallationPrice.HasValue, () => RuleFor(x => x.TotalInstallationPrice!.Value).GreaterThan(0));

        When(x => x.Notes is not null, () => RuleFor(x => x.Notes!).MaximumLength(1000));
    }
}

public class UpdateOfferPdfDtoValidator : AbstractValidator<UpdateOfferPdfDto>
{
    public UpdateOfferPdfDtoValidator()
    {
        RuleFor(x => x.OfferId).NotEmpty();
        RuleFor(x => x.OfferPdfPath).NotEmpty().MaximumLength(500);
    }
}

public class ApproveOfferDtoValidator : AbstractValidator<ApproveOfferDto>
{
    public ApproveOfferDtoValidator()
    {
        RuleFor(x => x.OfferId).NotEmpty();
        // IsAccepted is a boolean; no range validation needed.
        When(x => x.Notes is not null, () => RuleFor(x => x.Notes!).MaximumLength(1000));
    }
}

public class CreateInspectionProjectDtoValidator : AbstractValidator<CreateInspectionProjectDto>
{
    public CreateInspectionProjectDtoValidator()
    {
        RuleFor(x => x.ProjectAddress).NotEmpty().MaximumLength(256);
        RuleFor(x => x.PitType).NotEmpty().MaximumLength(256);

        RuleFor(x => x.PitWidth).GreaterThan(0);
        RuleFor(x => x.PitDepth).GreaterThan(0);
        RuleFor(x => x.LastFloorHeight).GreaterThan(0);
        RuleFor(x => x.HoleDepth).GreaterThan(0);
        RuleFor(x => x.TravelLength).GreaterThan(0);

        When(x => x.GoogleMapsLink is not null, () => RuleFor(x => x.GoogleMapsLink!).MaximumLength(500));
        When(x => x.Notes is not null, () => RuleFor(x => x.Notes!).MaximumLength(2000));

        // Either an existing customer id OR customer details must be provided.
        When(x => x.CustomerId is null, () =>
        {
            RuleFor(x => x.CustomerName).NotEmpty().MaximumLength(256);
            RuleFor(x => x.CustomerPhone).NotEmpty().MaximumLength(256);
            RuleFor(x => x.CustomerAddress).NotEmpty().MaximumLength(256);
            RuleFor(x => x.CustomerEmail).NotEmpty().EmailAddress().MaximumLength(256);
        });
    }
}

public class QuotationAttachmentDtoValidator : AbstractValidator<QuotationAttachmentDto>
{
    public QuotationAttachmentDtoValidator()
    {
        RuleFor(x => x.FileName).NotEmpty().MaximumLength(256);
        RuleFor(x => x.FilePath).NotEmpty().MaximumLength(512);
        RuleFor(x => x.ContentType).NotEmpty().MaximumLength(256);
        RuleFor(x => x.FileSize).GreaterThanOrEqualTo(0);
    }
}

public class CreateQuotationDtoValidator : AbstractValidator<CreateQuotationDto>
{
    public CreateQuotationDtoValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.DurationDays).GreaterThan(0);

        When(x => x.DurationNotes is not null, () => RuleFor(x => x.DurationNotes!).MaximumLength(256));
        When(x => x.Notes is not null, () => RuleFor(x => x.Notes!).MaximumLength(2000));

        RuleForEach(x => x.Attachments).SetValidator(new QuotationAttachmentDtoValidator());
    }
}

public class ApproveRejectQuotationDtoValidator : AbstractValidator<ApproveRejectQuotationDto>
{
    public ApproveRejectQuotationDtoValidator()
    {
        RuleFor(x => x.QuotationId).NotEmpty();
        When(x => x.Notes is not null, () => RuleFor(x => x.Notes!).MaximumLength(2000));
    }
}

