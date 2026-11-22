using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Models;
using Shouldly;

namespace Portfolio.Api.Tests.WaysOfWorking.Shared.Domain.Models;

public class IllustrationShould
{
    [Theory]
    [InlineData("tested", "tested")]
    [InlineData("  fast-response  ", "fast-response")]
    [InlineData("scene-2", "scene-2")]
    public void BeCreated(string givenIllustration, string expectedIllustration)
    {
        Illustration.Create(givenIllustration)
            .Match(
                illustration => illustration.ToString().ShouldBe(expectedIllustration),
                error => error.ShouldBeNull()
            );
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void NotAllowEmptyValue(string givenIllustration)
    {
        Illustration.Create(givenIllustration)
            .Match(
                illustration => illustration.ShouldBeNull(),
                error => error.ShouldBeOfType<ValidationError>()
                    .Message.ShouldBe("Illustration cannot be empty")
            );
    }

    [Fact]
    public void DoNotCreateIfTheCharacterLimitIsExceeded()
    {
        var givenIllustration = new string('a', 31);

        Illustration.Create(givenIllustration)
            .Match(
                illustration => illustration.ShouldBeNull(),
                error => error.ShouldBeOfType<ValidationError>()
                    .Message.ShouldBe("The illustration cannot exceed 30 characters")
            );
    }

    [Theory]
    [InlineData("Tested")]
    [InlineData("fast response")]
    [InlineData("-tested")]
    [InlineData("tested-")]
    [InlineData("fast--response")]
    [InlineData("pruébalo")]
    public void RejectKeysWithAnotherFormat(string givenIllustration)
    {
        Illustration.Create(givenIllustration)
            .Match(
                illustration => illustration.ShouldBeNull(),
                error => error.ShouldBeOfType<ValidationError>()
                    .Message.ShouldBe(
                        "The illustration can only contain lowercase letters, digits and hyphens between words"
                    )
            );
    }
}
