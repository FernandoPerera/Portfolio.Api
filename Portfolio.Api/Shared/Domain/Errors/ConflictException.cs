namespace Portfolio.Api.Shared.Domain.Errors;

public class ConflictException(string message) : Exception(message);
