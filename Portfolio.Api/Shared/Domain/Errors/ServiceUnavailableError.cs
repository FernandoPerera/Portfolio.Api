namespace Portfolio.Api.Shared.Domain.Errors;

public record ServiceUnavailableError(string Message, Exception? Cause = null) : DomainError(Message);
