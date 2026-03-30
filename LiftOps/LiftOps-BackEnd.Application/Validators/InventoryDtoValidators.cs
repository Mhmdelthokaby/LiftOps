using FluentValidation;
using LiftOps_BackEnd.Application.DTOs;
using static LiftOps_BackEnd.Application.Validators.ValidationConstants;

namespace LiftOps_BackEnd.Application.Validators;

public class UpdateInventoryItemDtoValidator : AbstractValidator<UpdateInventoryItemDto>
{
    public UpdateInventoryItemDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(Max256);
        RuleFor(x => x.ItemNumber).NotEmpty().MaximumLength(Max256);
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.UnitPrice).GreaterThan(0);
        RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.SupplierName).NotEmpty().MaximumLength(Max256);
    }
}

