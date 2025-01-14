using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Kjkardum.CloudyBack.Application.Behaviours;

public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseRequest
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var validationResults = await ExecuteValidators(request, cancellationToken).ConfigureAwait(false);
            ThrowValidationExceptionOnValidatonFailures(validationResults);
        }

        return await next().ConfigureAwait(false);
    }

    private async Task<ValidationResult[]> ExecuteValidators(TRequest request, CancellationToken cancellationToken)
    {
        var validationContext = new ValidationContext<TRequest>(request);

        var validationExecutor = (IValidator<TRequest> validator)
            => validator.ValidateAsync(validationContext, cancellationToken);
        var validationExecutions = _validators.Select(validationExecutor);

        var validationResults = await Task.WhenAll(validationExecutions).ConfigureAwait(false);
        return validationResults;
    }

    private static void ThrowValidationExceptionOnValidatonFailures(ValidationResult[] validationResults)
    {
        var failures = FilterForValidationFailures(validationResults);
        if (failures.Any())
        {
            throw new ValidationException(failures);
        }
    }

    private static IEnumerable<ValidationFailure> FilterForValidationFailures(ValidationResult[] validationResults)
        => validationResults
            .SelectMany(result => result.Errors)
            .Where(failure => failure is not null);
}
