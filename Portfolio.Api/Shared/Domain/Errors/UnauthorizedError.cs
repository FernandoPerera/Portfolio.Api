namespace Portfolio.Api.Shared.Domain.Errors;

public record UnauthorizedError(string Message) : DomainError(Message);
