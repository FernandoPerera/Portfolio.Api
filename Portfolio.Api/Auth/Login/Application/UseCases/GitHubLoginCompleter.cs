using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Auth.Login.Application.Dtos;
using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Auth.Shared.Domain.Ports;

namespace Portfolio.Api.Auth.Login.Application.UseCases;

public class GitHubLoginCompleter(
    IExternalIdentityProvider identityProvider,
    IAdminAuthorizer adminAuthorizer,
    ILoginCodeIssuer loginCodeIssuer,
    ILoginCodeSaverRepository loginCodeSaver
)
{
    public async Task<Either<Error, LoginCode>> Execute(GitHubCallbackInfo callbackInfo)
    {
        return await (
            from state in OAuthState.Create(callbackInfo.State).ToAsync()
            from expectedState in OAuthState.Create(callbackInfo.ExpectedState).ToAsync()
            from _ in state.EnsureMatches(expectedState).ToAsync()
            from authorizationCode in AuthorizationCode.Create(callbackInfo.Code).ToAsync()
            from userId in identityProvider.Identify(authorizationCode).ToAsync()
            from __ in adminAuthorizer.EnsureIsAdmin(userId).ToAsync()
            from loginCode in loginCodeIssuer.Issue().ToAsync()
            from ___ in loginCodeSaver.Save(loginCode).ToAsync()
            select loginCode
        );
    }
}
