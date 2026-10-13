using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Auth.Login.Application.Dtos;
using Portfolio.Api.Auth.Shared.Domain.Ports;

namespace Portfolio.Api.Auth.Login.Application.UseCases;

public class GitHubLoginStarter(
    IOAuthStateIssuer stateIssuer,
    IAuthorizationUrlBuilder authorizationUrlBuilder
)
{
    public Either<Error, GitHubLoginStart> Execute() =>
        from state in stateIssuer.Issue()
        select new GitHubLoginStart(state, authorizationUrlBuilder.Build(state));
}
