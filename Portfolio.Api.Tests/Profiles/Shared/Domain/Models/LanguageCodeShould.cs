using Portfolio.Api.Profiles.Shared.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;
using Shouldly;

namespace Portfolio.Api.Tests.Profiles.Shared.Domain.Models;

public class LanguageCodeShould
{
    [Theory]
    [InlineData("es")]
    [InlineData("en")]
    [InlineData("FR")]
    public void BeCreated(string givenLanguageCode)
    {
        LanguageCode.Create(givenLanguageCode)
            .Match(
                languageCode => languageCode.ToString().ShouldBe(givenLanguageCode.Trim().ToLowerInvariant()),
                error => error.ShouldBeNull()
            );
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void NotAllowEmptyValue(string givenLanguageCode)
    {
        LanguageCode.Create(givenLanguageCode)
            .Match(
                languageCode => languageCode.ShouldBeNull(),
                error => error.ShouldBeOfType<ValidationError>()
                    .Message.ShouldBe("Language code cannot be empty")
            );
    }

    [Theory]
    [InlineData("e")]
    [InlineData("esp")]
    [InlineData("espanol")]
    public void NotAllowInvalidLength(string givenLanguageCode)
    {
        LanguageCode.Create(givenLanguageCode)
            .Match(
                languageCode => languageCode.ShouldBeNull(),
                error => error.ShouldBeOfType<ValidationError>()
                    .Message.ShouldBe("Language code must have 2 letters")
            );
    }

    [Theory]
    [InlineData("12")]
    [InlineData("e1")]
    [InlineData("e#")]
    [InlineData("!s")]
    public void NotAllowNonAlphabeticCharacters(string givenLanguageCode)
    {
        LanguageCode.Create(givenLanguageCode)
            .Match(
                languageCode => languageCode.ShouldBeNull(),
                error => error.ShouldBeOfType<ValidationError>()
                    .Message.ShouldBe("Language code must have 2 letters")
            );
    }
}
