namespace Portfolio.Api.Shared.Domain.Errors;

public record NotFoundError(string Message) : DomainError(Message);
