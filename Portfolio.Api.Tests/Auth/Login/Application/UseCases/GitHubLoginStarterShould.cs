using LanguageExt;
using NSubstitute;
using Portfolio.Api.Auth.Login.Application.UseCases;
using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Auth.Shared.Domain.Ports;
using Portfolio.Api.Tests.Auth.Shared.Domain.Builders;
using Shouldly;

namespace Portfolio.Api.Tests.Auth.Login.Application.UseCases;

public class GitHubLoginStarterShould
{
    private readonly IOAuthStateIssuer _stateIssuer = Substitute.For<IOAuthStateIssuer>();
    private readonly IAuthorizationUrlBuilder _authorizationUrlBuilder = Substitute.For<IAuthorizationUrlBuilder>();
    private readonly GitHubLoginStarter _starter;

    public GitHubLoginStarterShould()
    {
        _starter = new GitHubLoginStarter(_stateIssuer, _authorizationUrlBuilder);
    }

    [Fact]
    public void StartTheLoginWithANewState()
    {
        var state = OAuthStateBuilder.Build();
        var authorizationUrl = new Uri("https://github.test/login/oauth/authorize");
        _stateIssuer.Issue().Returns(state);
        _authorizationUrlBuilder.Build(Arg.Any<OAuthState>()).Returns(authorizationUrl);

        _starter.Execute().Match(
            start =>
            {
                start.State.ShouldBe(state);
                start.AuthorizationUrl.ShouldBe(authorizationUrl);
                _authorizationUrlBuilder.Received(1).Build(state);
            },
            error => error.ShouldBeNull()
        );
    }
}
