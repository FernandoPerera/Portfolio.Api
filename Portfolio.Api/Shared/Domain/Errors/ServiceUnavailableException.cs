namespace Portfolio.Api.Shared.Domain.Errors;

public class ServiceUnavailableException(string message, Exception? innerException = null) : Exception(message, innerException);
