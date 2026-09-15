using Portfolio.Api.Profiles.Create.Domain.Models;
using Shouldly;

namespace Portfolio.Api.Tests.Profiles.Create.Domain.Models;

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
}