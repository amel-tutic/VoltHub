using FluentValidation;
using MediatR;

namespace VoltHub.Application.Common.Behaviors;

// Runs before every handler: if any validator for the request fails, the handler never executes.
internal sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var results = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            var failures = results.SelectMany(r => r.Errors).ToList();

            if (failures.Count > 0)
                throw new ValidationException(failures);   // becomes a 400 response with per-field errors
        }

        return await next(cancellationToken);
    }
}
