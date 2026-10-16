using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Auth.Refresh.Application.Dtos;
using Portfolio.Api.Auth.Shared.Domain.Errors;
using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Auth.Shared.Domain.Ports;

namespace Portfolio.Api.Auth.Refresh.Application.UseCases;

public class TokenRefresher(
    IRefreshTokenHasher refreshTokenHasher,
    IRefreshTokenFinderRepository refreshTokenFinder,
    IRefreshTokenConsumerRepository refreshTokenConsumer,
    IRefreshTokenRevokerRepository refreshTokenRevoker,
    IAccessTokenIssuer accessTokenIssuer,
    IRefreshTokenIssuer refreshTokenIssuer,
    IRefreshTokenSaverRepository refreshTokenSaver,
    TimeProvider timeProvider
)
{
    public async Task<Either<Error, AuthTokens>> Execute(RefreshInfo refreshInfo)
    {
        return await (
            from currentRefreshToken in RefreshToken.Create(refreshInfo.RefreshToken).ToAsync()
            from currentTokenHash in refreshTokenHasher.Hash(currentRefreshToken).ToAsync()
            from stored in refreshTokenFinder.Find(currentTokenHash).ToAsync()
            from consumed in Consume(stored).ToAsync()
            from accessToken in accessTokenIssuer.Issue(consumed.GetOwnerUsername()).ToAsync()
            from newRefreshToken in refreshTokenIssuer.Issue().ToAsync()
            from __ in refreshTokenSaver.Save(newRefreshToken, consumed.GetOwnerUsername()).ToAsync()
            select new AuthTokens(accessToken, newRefreshToken)
        );
    }

    private async Task<Either<Error, StoredRefreshToken>> Consume(StoredRefreshToken stored) =>
        await (
            from consumedRefreshToken in stored.Consume(timeProvider.GetUtcNow()).ToAsync()
            from _ in refreshTokenConsumer.Consume(consumedRefreshToken).ToAsync()
            select consumedRefreshToken
        ).MapLeftAsync(async error =>
        {
            if (error.Exception.Case is RefreshTokenReuseException)
            {
                await refreshTokenRevoker.Revoke(stored);
            }

            return error;
        });
}
