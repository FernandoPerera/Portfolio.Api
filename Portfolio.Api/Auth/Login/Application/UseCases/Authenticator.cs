using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Auth.Login.Application.Dtos;
using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Auth.Shared.Domain.Ports;

namespace Portfolio.Api.Auth.Login.Application.UseCases;

public class Authenticator(
    ICredentialsVerifier credentialsVerifier,
    IAccessTokenIssuer accessTokenIssuer,
    IRefreshTokenIssuer refreshTokenIssuer,
    IRefreshTokenSaverRepository refreshTokenSaver
)
{
    public async Task<Either<Error, AuthTokens>> Execute(LoginInfo loginInfo)
    {
        return await (
            from credentials in AdminCredentials.Create(loginInfo.Username, loginInfo.Password).ToAsync()
            from _ in credentialsVerifier.Verify(credentials).ToAsync()
            from accessToken in accessTokenIssuer.Issue(credentials.GetUsername()).ToAsync()
            from refreshToken in refreshTokenIssuer.Issue().ToAsync()
            from __ in refreshTokenSaver.Save(refreshToken).ToAsync()
            select new AuthTokens(accessToken, refreshToken)
        );
    }
}
