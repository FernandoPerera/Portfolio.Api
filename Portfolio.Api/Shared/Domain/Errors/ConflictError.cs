namespace Portfolio.Api.Shared.Domain.Errors;

public record ConflictError(string Message) : DomainError(Message);
