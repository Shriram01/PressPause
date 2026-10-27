using FluentValidation;
using PressPause.Application.Common.Exceptions;
using System.Text.Json;

namespace PressPause.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
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
            context.Response.StatusCode =
                StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            var response = new
            {
                status = 400,
                title = "Validation Failed",
                errors = ex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray())
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response),
                context.RequestAborted);
        }
        catch (KeyNotFoundException ex)
        {
            context.Response.StatusCode =
                StatusCodes.Status404NotFound;
            context.Response.ContentType = "application/json";

            var response = new
            {
                status = 404,
                title = ex.Message
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response),
                context.RequestAborted);
        }
        catch (ConflictException ex)
        {
            context.Response.StatusCode =
                StatusCodes.Status409Conflict;
            context.Response.ContentType = "application/json";

            var response = new
            {
                status = 409,
                title = ex.Message
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response),
                context.RequestAborted);
        }
        catch (UnauthorizedAccessException)
        {
            context.Response.StatusCode =
                StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";

            var response = new
            {
                status = 403,
                title = "You do not have permission to perform this action."
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response),
                context.RequestAborted);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An unhandled exception occurred while processing {Method} {Path}.",
                context.Request.Method,
                context.Request.Path);

            context.Response.StatusCode =
                StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new
            {
                status = 500,
                title = "An unexpected error occurred."
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response),
                context.RequestAborted);
        }
    }
}