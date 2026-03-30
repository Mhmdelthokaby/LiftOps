using FluentValidation;
using LiftOps_BackEnd.Application.Features.Admins.DTOs;
using static LiftOps_BackEnd.Application.Validators.ValidationConstants;

namespace LiftOps_BackEnd.Application.Validators;

public class UpdateAdminDtoValidator : AbstractValidator<UpdateAdminDto>
{
    public UpdateAdminDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(Max256);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(Max256);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(Max256);

        When(x => x.Password is not null, () =>
        {
            RuleFor(x => x.Password!)
                .NotEmpty()
                .MinimumLength(6)
                .MaximumLength(Max256);
        });
    }
}

