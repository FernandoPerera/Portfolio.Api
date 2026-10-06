using Portfolio.Api.Profiles.Shared.Domain.Models;
using Shouldly;

namespace Portfolio.Api.Tests.Profiles.Shared.Domain.Models;

public class EnterpriseNameShould
{
    [Fact]
    public void BeCreated()
    {
        const string givenEnterprise = "Lean Mind";

        EnterpriseName.Create(givenEnterprise).Match(
            name => name.ToString().ShouldBe(givenEnterprise),
            error => error.ShouldBeNull()
        );
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void BeCreatedWithEmptyValues(string? givenEnterpriseName)
    {
        EnterpriseName.Create(givenEnterpriseName).Match(
            name => name.ToString().ShouldBe(string.Empty),
            error => error.ShouldBeNull()
        );
    }
}