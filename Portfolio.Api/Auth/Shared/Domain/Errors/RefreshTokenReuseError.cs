using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Auth.Shared.Domain.Errors;

public record RefreshTokenReuseError(string Message) : UnauthorizedError(Message);
