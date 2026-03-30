using FluentValidation;
using LiftOps_BackEnd.Application.DTOs.Installation;
using static LiftOps_BackEnd.Application.Validators.ValidationConstants;

namespace LiftOps_BackEnd.Application.Validators;

public class CreateTechnicianDtoValidator : AbstractValidator<CreateTechnicianDto>
{
    public CreateTechnicianDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(Max256);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(Max256);
        When(x => x.Specialization is not null, () => RuleFor(x => x.Specialization!).MaximumLength(Max256));
        When(x => x.Username is not null, () => RuleFor(x => x.Username!).MaximumLength(Max256));

        When(x => x.Password is not null, () =>
        {
            RuleFor(x => x.Password!)
                .NotEmpty()
                .MinimumLength(6)
                .MaximumLength(Max256);
        });
    }
}

