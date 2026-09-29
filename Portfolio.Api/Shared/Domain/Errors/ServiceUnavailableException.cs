namespace Portfolio.Api.Shared.Domain.Errors;

public class ServiceUnavailableException(string message) : Exception(message);