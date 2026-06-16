using FluentValidation;
using Flora.Cms.Application.Abstractions.Messaging;
using ValidationException = Flora.Cms.Application.Common.Exceptions.ValidationException;

namespace Flora.Cms.Application.Common.Behaviors;

/// <summary>
/// Runs every FluentValidation validator registered for the request before the
/// handler executes. On failure it throws the application's
/// <see cref="ValidationException"/> (translated to HTTP 400 by the API), so no
/// handler ever has to defensively re-check its input.
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        if (!validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var results = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, ct)));

        var failures = results
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count > 0)
        {
            var errors = failures
                .GroupBy(f => f.PropertyName, f => f.ErrorMessage)
                .ToDictionary(g => g.Key, g => g.Distinct().ToArray());

            throw new ValidationException(errors);
        }

        return await next();
    }
}
