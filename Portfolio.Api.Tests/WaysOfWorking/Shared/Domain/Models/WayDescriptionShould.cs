using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Models;
using Shouldly;

namespace Portfolio.Api.Tests.WaysOfWorking.Shared.Domain.Models;

public class WayDescriptionShould
{
    [Theory]
    [InlineData("Claro", "Claro")]
    [InlineData("  Claro  ", "Claro")]
    public void BeCreated(string givenDescription, string expectedDescription)
    {
        WayDescription.Create(givenDescription)
            .Match(
                value => value.ToString().ShouldBe(expectedDescription),
                error => error.ShouldBeNull()
            );
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void NotAllowEmptyValue(string givenDescription)
    {
        WayDescription.Create(givenDescription)
            .Match(
                value => value.ShouldBeNull(),
                error => error.ShouldBeOfType<ValidationError>()
                    .Message.ShouldBe("Description cannot be empty")
            );
    }

    [Fact]
    public void DoNotCreateIfTheCharacterLimitIsExceeded()
    {
        var givenDescription = new string('a', 500 + 1);

        WayDescription.Create(givenDescription)
            .Match(
                value => value.ShouldBeNull(),
                error => error.ShouldBeOfType<ValidationError>()
                    .Message.ShouldBe("The description cannot exceed 500 characters")
            );
    }
}
