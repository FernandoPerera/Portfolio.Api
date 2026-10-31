using Ci.Languages.Create.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;
using Shouldly;

namespace Portfolio.Api.Tests.Languages.Create.Domain.Models;

public class LanguageNameShould
{
    [Fact]
    public void BeCreated()
    {
        const string givenName = "Español";

        LanguageName.Create(givenName)
            .Match(
                name => name.ToString().ShouldBe(givenName),
                error => error.ShouldBeNull()
            );
    }

    [Fact]
    public void TrimSurroundingSpaces()
    {
        LanguageName.Create("  English  ")
            .Match(
                name => name.ToString().ShouldBe("English"),
                error => error.ShouldBeNull()
            );
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void NotAllowEmptyValue(string givenName)
    {
        LanguageName.Create(givenName)
            .Match(
                name => name.ShouldBeNull(),
                error => error.ShouldBeOfType<ValidationError>()
                    .Message.ShouldBe("Language name cannot be empty")
            );
    }

    [Fact]
    public void DoNotCreateIfTheCharacterLimitIsExceeded()
    {
        var givenName = new string('a', 21);

        LanguageName.Create(givenName)
            .Match(
                name => name.ShouldBeNull(),
                error => error.ShouldBeOfType<ValidationError>()
                    .Message.ShouldBe("The language name cannot exceed 20 characters")
            );
    }
}
