using LanguageExt;
using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Tests.Auth.Shared.Domain.Builders;
using Portfolio.Api.Tests.Shared.Extensions;
using Shouldly;

namespace Portfolio.Api.Tests.Auth.Shared.Domain.Models;

public class OAuthStateShould
{
    [Theory]
    [InlineData("value", "value")]
    [InlineData("  value  ", "value")]
    public void BeCreated(string given, string expected)
    {
        OAuthState.Create(given)
            .Match(
                created => created.ToString().ShouldBe(expected),
                error => error.ShouldBeNull()
            );
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void NotAllowEmptyValue(string given)
    {
        OAuthState.Create(given)
            .Match(
                created => created.ShouldBeNull(),
                error => error.ShouldBeOfType<ValidationError>()
                    .Message.ShouldBe("OAuth state cannot be empty")
            );
    }

    [Fact]
    public void MatchTheSameState()
    {
        OAuthStateBuilder.Build("state").EnsureMatches(OAuthStateBuilder.Build("state"))
            .Match(
                unit => unit.ShouldBeUnit(),
                error => error.ShouldBeNull()
            );
    }

    [Theory]
    [InlineData("other-state")]
    [InlineData("stat")]
    public void RejectADifferentState(string other)
    {
        OAuthStateBuilder.Build("state").EnsureMatches(OAuthStateBuilder.Build(other))
            .Match(
                unit => unit.ShouldBeEquivalentTo(null),
                error => error.ShouldBeOfType<UnauthorizedError>()
                    .Message.ShouldBe("Invalid OAuth state")
            );
    }
}
