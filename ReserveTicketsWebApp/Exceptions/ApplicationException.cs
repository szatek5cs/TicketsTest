using Microsoft.AspNetCore.Identity;

namespace ReserveTicketsWebApp.Exceptions;

public class ApplicationException(string message) : Exception(message);

public class NotFoundException(string name, object key) 
    : ApplicationException($"{name} with id ({key}) was not found.");

public class ConflictException(string message) 
    : ApplicationException(message);

public class BadRequestException(string message) 
    : ApplicationException(message);

public class DomainValidationException(string message, string errorCode) 
    : ApplicationException(message)
{
    public string Code { get; } = errorCode;
}

public class ValidationAppException(IDictionary<string, string[]> errors) 
    : ApplicationException("One or more validation failures have occurred.")
{
    public IDictionary<string, string[]> Errors { get; } = errors;

    // Helper dla Identity
    public static ValidationAppException FromIdentityResult(IdentityResult result)
    {
        var errors = result.Errors
            .GroupBy(e => e.Code)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.Description).ToArray()
            );
        return new ValidationAppException(errors);
    }
}