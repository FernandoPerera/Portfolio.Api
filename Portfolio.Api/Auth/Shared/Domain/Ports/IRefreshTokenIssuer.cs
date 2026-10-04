using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Auth.Shared.Domain.Models;

namespace Portfolio.Api.Auth.Shared.Domain.Ports;

public interface IRefreshTokenIssuer
{
    Either<Error, RefreshToken> Issue();
}
