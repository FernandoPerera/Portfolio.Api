using Portfolio.Api.Auth.Shared.Infrastructure.Adapters;
using Shouldly;

namespace Portfolio.Api.Tests.Auth.Shared.Infrastructure.Adapters;

public class RandomLoginCodeIssuerShould
{
    private readonly RandomLoginCodeIssuer _issuer = new();

    [Fact]
    public void IssueADifferentCodeEachTime()
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
