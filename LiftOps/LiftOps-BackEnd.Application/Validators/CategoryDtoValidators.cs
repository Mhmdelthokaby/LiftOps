using FluentValidation;
using LiftOps_BackEnd.Application.DTOs;
using static LiftOps_BackEnd.Application.Validators.ValidationConstants;

namespace LiftOps_BackEnd.Application.Validators;

public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
{
    public CreateCategoryDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(Max256);
        When(x => x.Description is not null, () => RuleFor(x => x.Description!).MaximumLength(Max2000));
    }
}

