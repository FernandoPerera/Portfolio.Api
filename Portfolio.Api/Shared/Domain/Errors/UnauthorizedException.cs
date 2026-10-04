namespace Portfolio.Api.Shared.Domain.Errors;

public class UnauthorizedException(string message) : Exception(message);
