using System.Data;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ReserveTicketsWebApp.Exceptions;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger = logger;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        if (exception is ValidationAppException validationException)
        {
            var validatonProblemDetails = new ValidationProblemDetails(validationException.Errors)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation Error",
                Detail = validationException.Message
            };
            
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await httpContext.Response.WriteAsJsonAsync(validatonProblemDetails, cancellationToken);

            var errorSummary = string.Join(", ", validationException.Errors
                .Select(e => $"{e.Key}: {string.Join("; ", e.Value)}"));
            _logger.LogWarning("Validation Error: {ValidationMessage}, Details: {ValidationErrors}", exception.Message, errorSummary);
            return true;
        }
        
        if (exception is DbUpdateConcurrencyException)
        {
            var concurencyProblemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Conflict",
                Detail = "The resource you are trying to update has been modified by another process. Please refresh and try again.",
                Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
            };

            httpContext.Response.StatusCode = StatusCodes.Status409Conflict;
            await httpContext.Response.WriteAsJsonAsync(concurencyProblemDetails, cancellationToken);

            _logger.LogWarning(exception, "Concurrency conflict: {Message}", exception.Message);
            return true;
        }

        var (statusCode, title) = exception switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, "Not Found"),
            ConflictException => (StatusCodes.Status409Conflict, "Conflict"),
            DomainValidationException => (StatusCodes.Status422UnprocessableEntity, "Business Rule Violation"),
            System.Text.Json.JsonException => (StatusCodes.Status400BadRequest, "Invalid JSON Format"),
            BadHttpRequestException => (StatusCodes.Status400BadRequest, "Invalid Request Format"),
            BadRequestException => (StatusCodes.Status400BadRequest, "Bad Request"),
            _ => (StatusCodes.Status500InternalServerError, "Server Error")
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = exception.Message,
            Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
        };

        if (exception is DomainValidationException domainEx)
        {
            // Dodajemy kod błędu do słownika Extensions
            problemDetails.Extensions.Add("errorCode", domainEx.Code);
        }

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        if (statusCode == 500)
        {
            _logger.LogError(exception, "Unhandled server error: {Message}", exception.Message);
        }
        else
        {
            _logger.LogWarning("Client error ({StatusCode}): {Message}", statusCode, exception.Message);
        }

        return true; // Przerwij potok - obsłużyliśmy błąd
    }
}