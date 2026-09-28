namespace Portfolio.Api.Shared.Domain.Errors;

public class NotFoundException(string message) : Exception(message);
