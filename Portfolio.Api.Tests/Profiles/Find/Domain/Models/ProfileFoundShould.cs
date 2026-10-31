using Portfolio.Api.Profiles.Shared.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Tests.Profiles.Find.Domain.Builders;
using Shouldly;

namespace Portfolio.Api.Tests.Profiles.Find.Domain.Models;

public class ProfileFoundShould
{
    [Fact]
    public void ReturnTheTranslationOfTheGivenLanguage()
    {
        var spanish = ProfileTranslationFoundBuilder.Build("es");
        var english = ProfileTranslationFoundBuilder.Build("en", "Nice to meet you", "Software Engineer");
        var profile = ProfileFoundBuilder.Build(translations: [spanish, english]);
        var languageCode = LanguageCode.Create("en").IfLeft(error => throw new Exception(error.Message));

        profile.GetTranslation(languageCode)
            .Match(
                translation => translation.ShouldBe(english),
                error => error.ShouldBeNull()
            );
    }

    [Fact]
    public void FailWhenTheLanguageIsNotInTheProfile()
    {
        var profile = ProfileFoundBuilder.Build(translations: [ProfileTranslationFoundBuilder.Build("es")]);
        var languageCode = LanguageCode.Create("fr").IfLeft(error => throw new Exception(error.Message));

        profile.GetTranslation(languageCode)
            .Match(
                translation => translation.ShouldBeNull(),
                error => error.ShouldBeOfType<NotFoundError>()
                    .Message.ShouldBe("Profile translation for language 'fr' was not found")
            );
    }
}
