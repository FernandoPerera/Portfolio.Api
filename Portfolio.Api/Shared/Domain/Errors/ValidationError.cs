namespace Portfolio.Api.Shared.Domain.Errors;

public record ValidationError(string Message) : DomainError(Message);
