using LanguageExt;
using Portfolio.Api.Auth.Login.Application.Dtos;
using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Auth.Shared.Domain.Ports;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Auth.Login.Application.UseCases;

public class LoginCodeExchanger(
    ILoginCodeConsumerRepository loginCodeConsumer,
    IAdminUsernameProvider adminUsernameProvider,
    IAccessTokenIssuer accessTokenIssuer,
    IRefreshTokenIssuer refreshTokenIssuer,
    IRefreshTokenSaverRepository refreshTokenSaver,
    IExpiredRefreshTokenCleanerRepository expiredRefreshTokenCleaner
)
{
    public async Task<Either<DomainError, AuthTokens>> Execute(ExchangeInfo exchangeInfo)
    {
        return await (
            from loginCode in LoginCode.Create(exchangeInfo.Code).ToAsync()
            from _ in loginCodeConsumer.Consume(loginCode).ToAsync()
            from owner in adminUsernameProvider.Get().ToAsync()
            from accessToken in accessTokenIssuer.Issue(owner).ToAsync()
            from refreshToken in refreshTokenIssuer.Issue().ToAsync()
            from __ in refreshTokenSaver.Save(refreshToken, owner).ToAsync()
            from ___ in expiredRefreshTokenCleaner.Clean().ToAsync()
            select new AuthTokens(accessToken, refreshToken)
        );
    }
}
