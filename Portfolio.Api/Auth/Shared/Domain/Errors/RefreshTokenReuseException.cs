using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Auth.Shared.Domain.Errors;

public class RefreshTokenReuseException(string message) : UnauthorizedException(message);
