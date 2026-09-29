namespace Portfolio.Api.Shared.Domain.Errors;

public class ValidationException(string message): Exception(message);