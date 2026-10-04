using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Auth.Refresh.Application.Dtos;
using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Auth.Shared.Domain.Ports;

namespace Portfolio.Api.Auth.Refresh.Application.UseCases;

public class TokenRefresher(
    IRefreshTokenConsumerRepository refreshTokenConsumer,
    IAccessTokenIssuer accessTokenIssuer,
    IRefreshTokenIssuer refreshTokenIssuer,
    IRefreshTokenSaverRepository refreshTokenSaver
)
{
    public async Task<Either<Error, AuthTokens>> Execute(RefreshInfo refreshInfo)
    {
        return await (
            from currentRefreshToken in RefreshToken.Create(refreshInfo.RefreshToken).ToAsync()
            from owner in refreshTokenConsumer.Consume(currentRefreshToken).ToAsync()
            from accessToken in accessTokenIssuer.Issue(owner).ToAsync()
            from newRefreshToken in refreshTokenIssuer.Issue().ToAsync()
            from _ in refreshTokenSaver.Save(newRefreshToken, owner).ToAsync()
            select new AuthTokens(accessToken, newRefreshToken)
        );
    }
}
