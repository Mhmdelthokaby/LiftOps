using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace LiftOps_BackEnd.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IReadOnlyList<IValidator> _validators;
    private readonly IReadOnlyList<(Type ValidatedType, IValidator Validator)> _typedValidators;

    public ValidationBehavior(IEnumerable<IValidator> validators)
    {
        _validators = validators?.ToList() ?? new List<IValidator>();
        _typedValidators = _validators
            .Select(v =>
            {
                var validatorInterface = v.GetType()
                    .GetInterfaces()
                    .FirstOrDefault(i =>
                        i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>));

                if (validatorInterface == null)
                    return (ValidatedType: (Type?)null, Validator: v);

                var validatedType = validatorInterface.GetGenericArguments()[0];
                return (ValidatedType: validatedType, Validator: v);
            })
            .Where(x => x.ValidatedType != null)
            .Select(x => (x.ValidatedType!, x.Validator))
            .ToList();
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_typedValidators.Count == 0)
            return await next();

        // Validate the MediatR request itself and any DTO-like objects found on it.
        // This ensures FluentValidation rules run for DTO properties inside commands.
        var instancesToValidate = new List<object> { request! };

        var requestType = typeof(TRequest);
        foreach (var property in requestType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (!property.CanRead) continue;

            var value = property.GetValue(request);
            if (value is null) continue;
            if (value is string) continue; // Avoid treating string as IEnumerable<char>

            if (value is IEnumerable enumerable)
            {
                foreach (var element in enumerable)
                {
                    if (element is null) continue;
                    if (element is string) continue;
                    instancesToValidate.Add(element);
                }
            }
            else
            {
                instancesToValidate.Add(value);
            }
        }

        var failures = new List<ValidationFailure>();
        foreach (var instance in instancesToValidate)
        {
            foreach (var (validatedType, validator) in _typedValidators)
            {
                if (!validatedType.IsInstanceOfType(instance))
                    continue;

                var contextType = typeof(ValidationContext<>).MakeGenericType(validatedType);
                var context = Activator.CreateInstance(contextType, instance);
                if (context is null) continue;

                var validationResult = await validator.ValidateAsync((IValidationContext)context, cancellationToken);
                failures.AddRange(validationResult.Errors.Where(f => f != null));
            }
        }

        if (failures.Count != 0)
            throw new ValidationException(failures);

        return await next();
    }
}
