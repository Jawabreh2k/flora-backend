using System.Diagnostics;
using Flora.Cms.Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using ValidationException = Flora.Cms.Application.Common.Exceptions.ValidationException;

namespace Flora.Cms.Api.Middleware;

/// <summary>
/// Translates application exceptions into RFC-7807 problem responses, so handlers can
/// throw intent-revealing exceptions and controllers stay free of error plumbing:
/// <list type="bullet">
///   <item><see cref="ValidationException"/> → 400 (validation problem when field-keyed).</item>
///   <item><see cref="NotFoundException"/> → 404.</item>
///   <item><see cref="ConflictException"/> → 409.</item>
///   <item>anything else → 500.</item>
/// </list>
/// </summary>
public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            await WriteValidationAsync(context, ex);
        }
        catch (NotFoundException ex)
        {
            await WriteProblemAsync(context, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (ConflictException ex)
        {
            await WriteProblemAsync(context, StatusCodes.Status409Conflict, ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception processing {Method} {Path}", context.Request.Method, context.Request.Path);
            await WriteProblemAsync(context, StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
        }
    }

    private static Task WriteValidationAsync(HttpContext context, ValidationException ex)
    {
        // Field-keyed failures render as a validation problem whose envelope matches the
        // framework's built-in [ApiController] ModelState response (type + traceId), so
        // clients see one consistent shape regardless of which layer rejected the input.
        if (ex.Errors.Count > 0)
        {
            var problem = new ValidationProblemDetails(ex.Errors.ToDictionary(e => e.Key, e => e.Value))
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "One or more validation errors occurred.",
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            };
            problem.Extensions["traceId"] = Activity.Current?.Id ?? context.TraceIdentifier;
            return WriteAsync(context, StatusCodes.Status400BadRequest, problem);
        }

        return WriteProblemAsync(context, StatusCodes.Status400BadRequest, ex.Message);
    }

    private static Task WriteProblemAsync(HttpContext context, int statusCode, string title)
        => WriteAsync(context, statusCode, new ProblemDetails { Status = statusCode, Title = title });

    private static Task WriteAsync(HttpContext context, int statusCode, ProblemDetails problem)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";
        return context.Response.WriteAsJsonAsync(problem, problem.GetType());
    }
}
