using Portfolio.Api.Auth.Shared.Infrastructure.Adapters;
using Shouldly;

namespace Portfolio.Api.Tests.Auth.Shared.Infrastructure.Adapters;

public class RandomRefreshTokenIssuerShould
{
    private readonly RandomRefreshTokenIssuer _issuer = new();

    [Fact]
    public void IssueADifferentTokenEachTime()
    {
        _issuer.Issue().Match(
            first => _issuer.Issue().Match(
                second =>
                {
                    first.ToString().Length.ShouldBeGreaterThanOrEqualTo(80);
                    first.ToString().ShouldNotBe(second.ToString());
                },
                error => error.ShouldBeNull()
            ),
            error => error.ShouldBeNull()
        );
    }
}
