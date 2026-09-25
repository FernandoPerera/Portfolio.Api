using Portfolio.Api.Profiles.Find.Domain.Models;
using Shouldly;

namespace Portfolio.Api.Tests.Profiles.Find.Domain.Models;

public class EnterpriseNameShould
{
    [Fact]
    public void BeCreated()
    {
        const string givenEnterprise = "Lean Mind";

        EnterpriseName.Create(givenEnterprise).Match(
            enterprise => enterprise.ToString().ShouldBe(givenEnterprise),
            error => error.ShouldBeNull()
        );
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void BeEmptyWhenNameIsNullOrWhitespace(string? givenEnterprise)
    {
        EnterpriseName.Create(givenEnterprise).Match(
            enterprise => enterprise.ToString().ShouldBe(string.Empty),
            error => error.ShouldBeNull()
        );
    }
}
