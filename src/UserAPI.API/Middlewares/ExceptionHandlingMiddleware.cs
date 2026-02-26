namespace UserAPI.API.Middlewares;

using FluentValidation;
using UserAPI.Core.Domain.Exceptions;

/// <summary>
/// Global exception handling middleware
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error occurred");
            await HandleValidationExceptionAsync(context, ex);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Resource not found");
            await HandleNotFoundExceptionAsync(context, ex);
        }
        catch (DuplicateException ex)
        {
            _logger.LogWarning(ex, "Duplicate resource detected");
            await HandleConflictExceptionAsync(context, ex);
        }
        catch (AuthenticationException ex)
        {
            _logger.LogWarning(ex, "Authentication failed");
            await HandleAuthenticationExceptionAsync(context, ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access");
            await HandleUnauthorizedExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleValidationExceptionAsync(HttpContext context, ValidationException exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status400BadRequest;

        var errors = exception.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

        return context.Response.WriteAsJsonAsync(new { message = "Validation failed", errors });
    }

    private static Task HandleNotFoundExceptionAsync(HttpContext context, NotFoundException exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status404NotFound;

        return context.Response.WriteAsJsonAsync(new { message = exception.Message });
    }

    private static Task HandleConflictExceptionAsync(HttpContext context, DuplicateException exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status409Conflict;

        return context.Response.WriteAsJsonAsync(new { message = exception.Message });
    }

    private static Task HandleAuthenticationExceptionAsync(HttpContext context, AuthenticationException exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;

        return context.Response.WriteAsJsonAsync(new { message = exception.Message });
    }

    private static Task HandleUnauthorizedExceptionAsync(HttpContext context, UnauthorizedAccessException exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;

        return context.Response.WriteAsJsonAsync(new { message = exception.Message });
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        return context.Response.WriteAsJsonAsync(new { message = "An internal server error occurred" });
    }
}
