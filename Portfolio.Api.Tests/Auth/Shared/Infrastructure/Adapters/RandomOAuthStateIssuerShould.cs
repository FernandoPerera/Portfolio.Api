using Portfolio.Api.Auth.Shared.Infrastructure.Adapters;
using Shouldly;

namespace Portfolio.Api.Tests.Auth.Shared.Infrastructure.Adapters;

public class RandomOAuthStateIssuerShould
{
    private readonly RandomOAuthStateIssuer _issuer = new();

    [Fact]
    public void IssueADifferentStateEachTime()
    {
        _issuer.Issue().Match(
            first => _issuer.Issue().Match(
                second =>
                {
                    first.ToString().Length.ShouldBeGreaterThanOrEqualTo(40);
                    first.ToString().ShouldNotBe(second.ToString());
                },
                error => error.ShouldBeNull()
            ),
            error => error.ShouldBeNull()
        );
    }
}
