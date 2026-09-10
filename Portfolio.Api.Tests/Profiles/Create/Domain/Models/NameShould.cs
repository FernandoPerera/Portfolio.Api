using Portfolio.Api.Profiles.Create.Domain.Models;
using Shouldly;

namespace Portfolio.Api.Tests.Profiles.Create.Domain.Models;

public class NameShould
{
    [Fact]
    public void BeCreated()
    {
        const string givenName = "Fernando";

        Name.Create(givenName)
            .Match(
                name => name.ToString().ShouldBe(givenName),
                error => error.ShouldBeNull()
            );
    }
}