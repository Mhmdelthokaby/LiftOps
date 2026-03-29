using FluentValidation;
using LiftOps_BackEnd.Application.DTOs;

namespace LiftOps_BackEnd.Application.Validators;

public class CreateInventoryItemDtoValidator : AbstractValidator<CreateInventoryItemDto>
{
    public CreateInventoryItemDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Item name is required.");
        RuleFor(x => x.CategoryId).NotEmpty().WithMessage("CategoryId is required.");
        RuleFor(x => x.UnitPrice).GreaterThan(0).WithMessage("UnitPrice must be greater than zero.");
        RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0).WithMessage("StockQuantity cannot be negative.");
        RuleFor(x => x.SupplierName).NotEmpty().WithMessage("SupplierName is required.");
    }
}
