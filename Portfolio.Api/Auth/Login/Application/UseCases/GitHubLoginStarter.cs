using LanguageExt;
using Portfolio.Api.Auth.Login.Application.Dtos;
using Portfolio.Api.Auth.Shared.Domain.Ports;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Auth.Login.Application.UseCases;

public class GitHubLoginStarter(
    IOAuthStateIssuer stateIssuer,
    IAuthorizationUrlBuilder authorizationUrlBuilder
)
{
    public Either<DomainError, GitHubLoginStart> Execute() =>
        from state in stateIssuer.Issue()
        select new GitHubLoginStart(state, authorizationUrlBuilder.Build(state));
}
