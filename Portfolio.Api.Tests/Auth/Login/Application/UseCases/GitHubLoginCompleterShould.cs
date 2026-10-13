using LanguageExt;
using LanguageExt.Common;
using NSubstitute;
using Portfolio.Api.Auth.Login.Application.Dtos;
using Portfolio.Api.Auth.Login.Application.UseCases;
using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Auth.Shared.Domain.Ports;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Tests.Auth.Shared.Domain.Builders;
using Shouldly;

namespace Portfolio.Api.Tests.Auth.Login.Application.UseCases;

public class GitHubLoginCompleterShould
{
    private const string LoginCodeValue = "login-code";
    private const string State = "oauth-state";

    private readonly IExternalIdentityProvider _identityProvider = Substitute.For<IExternalIdentityProvider>();
    private readonly IAdminAuthorizer _adminAuthorizer = Substitute.For<IAdminAuthorizer>();
    private readonly ILoginCodeIssuer _loginCodeIssuer = Substitute.For<ILoginCodeIssuer>();
    private readonly ILoginCodeSaverRepository _loginCodeSaver = Substitute.For<ILoginCodeSaverRepository>();
    private readonly GitHubLoginCompleter _completer;

    public GitHubLoginCompleterShould()
    {
        _completer = new GitHubLoginCompleter(_identityProvider, _adminAuthorizer, _loginCodeIssuer, _loginCodeSaver);
        _identityProvider.Identify(Arg.Any<AuthorizationCode>()).Returns(ExternalUserIdBuilder.Build());
        _adminAuthorizer.EnsureIsAdmin(Arg.Any<ExternalUserId>()).Returns(Unit.Default);
        _loginCodeIssuer.Issue().Returns(LoginCodeBuilder.Build(LoginCodeValue));
        _loginCodeSaver.Save(Arg.Any<LoginCode>()).Returns(Unit.Default);
    }

    [Fact]
    public async Task IssueALoginCodeForTheAdmin()
    {
        var result = await _completer.Execute(new GitHubCallbackInfo("github-code", State, State));

        result.Match(
            loginCode =>
            {
                loginCode.ToString().ShouldBe(LoginCodeValue);
                _loginCodeSaver.Received(1).Save(Arg.Is<LoginCode>(saved => saved.ToString() == LoginCodeValue));
            },
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task NotIssueALoginCodeWhenTheUserIsNotTheAdmin()
    {
        _adminAuthorizer.EnsureIsAdmin(Arg.Any<ExternalUserId>())
            .Returns(Error.New(new ForbiddenException("This user is not allowed to manage the portfolio")));

        var result = await _completer.Execute(new GitHubCallbackInfo("github-code", State, State));

        result.Match(
            loginCode => loginCode.ShouldBeNull(),
            error =>
            {
                error.Exception.Case.ShouldBeOfType<ForbiddenException>();
                _loginCodeSaver.DidNotReceive().Save(Arg.Any<LoginCode>());
            }
        );
    }

    [Fact]
    public async Task NotCallGitHubWhenTheStateDoesNotMatch()
    {
        var result = await _completer.Execute(new GitHubCallbackInfo("github-code", State, "another-state"));

        result.Match(
            loginCode => loginCode.ShouldBeNull(),
            error =>
            {
                error.Exception.Case.ShouldBeOfType<UnauthorizedException>();
                _identityProvider.DidNotReceive().Identify(Arg.Any<AuthorizationCode>());
            }
        );
    }
}
