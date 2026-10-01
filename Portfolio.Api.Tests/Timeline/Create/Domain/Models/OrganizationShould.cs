using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Timeline.Create.Domain.Models;
using Shouldly;

namespace Portfolio.Api.Tests.Timeline.Create.Domain.Models;

public class OrganizationShould
{
    [Theory]
    [InlineData("Lean Mind", "Lean Mind")]
    [InlineData("  Lean Mind  ", "Lean Mind")]
    public void BeCreated(string givenOrganization, string expectedOrganization)
    {
        Organization.Create(givenOrganization)
            .Match(
                organization => organization.ToString().ShouldBe(expectedOrganization),
                error => error.ShouldBeNull()
            );
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void NotAllowEmptyValue(string givenOrganization)
    {
        Organization.Create(givenOrganization)
            .Match(
                organization => organization.ShouldBeNull(),
                error => error.Exception.Case.ShouldBeOfType<ValidationException>()
                    .Message.ShouldBe("Organization cannot be empty")
            );
    }

    [Fact]
    public void DoNotCreateIfTheCharacterLimitIsExceeded()
    {
        var givenOrganization = new string('a', 101);

        Organization.Create(givenOrganization)
            .Match(
                organization => organization.ShouldBeNull(),
                error => error.Exception.Case.ShouldBeOfType<ValidationException>()
                    .Message.ShouldBe("The organization cannot exceed 100 characters")
            );
    }
}
