using LanguageExt;
using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Auth.Shared.Domain.Ports;

public interface IRefreshTokenHasher
{
    Either<DomainError, RefreshTokenHash> Hash(RefreshToken refreshToken);
}
