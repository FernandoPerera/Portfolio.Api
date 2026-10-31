namespace Portfolio.Api.Shared.Domain.Errors;

public record ForbiddenError(string Message) : DomainError(Message);
