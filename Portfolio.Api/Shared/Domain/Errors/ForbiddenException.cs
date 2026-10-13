namespace Portfolio.Api.Shared.Domain.Errors;

public class ForbiddenException(string message) : Exception(message);
