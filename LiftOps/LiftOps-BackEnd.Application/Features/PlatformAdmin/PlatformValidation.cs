using FluentValidation;
using FluentValidation.Results;

namespace LiftOps_BackEnd.Application.Features.PlatformAdmin;

internal static class PlatformValidation
{
    public static void Throw(string message) =>
        throw new ValidationException(new[] { new ValidationFailure(string.Empty, message) });
}
