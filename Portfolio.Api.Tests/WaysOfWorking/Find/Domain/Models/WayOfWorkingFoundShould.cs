using Portfolio.Api.Profiles.Shared.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Tests.Shared.Extensions;
using Portfolio.Api.Tests.WaysOfWorking.Find.Domain.Builders;
using Portfolio.Api.Tests.WaysOfWorking.Shared.Domain.Builders;
using Portfolio.Api.WaysOfWorking.Find.Domain.Models;
using Shouldly;

namespace Portfolio.Api.Tests.WaysOfWorking.Find.Domain.Models;

public class WayOfWorkingFoundShould
{
    private readonly WayOfWorkingFound _way =
        WayOfWorkingFoundBuilder.Build(
            translations: [WayTranslationBuilder.Build("es"), WayTranslationBuilder.Build("en")]
        );

    [Fact]
    public void AcceptATranslationInANewLanguage()
    {
        var languageCode = LanguageCode.Create("fr").IfLeft(error => throw new Exception(error.Message));

        _way.EnsureTranslationIsNew(languageCode)
            .Match(
                unit => unit.ShouldBeUnit(),
                error => error.ShouldBeNull()
            );
    }

    [Fact]
    public void RejectATranslationInALanguageItAlreadyHas()
    {
        var languageCode = LanguageCode.Create("es").IfLeft(error => throw new Exception(error.Message));

        _way.EnsureTranslationIsNew(languageCode)
            .Match(
                unit => unit.ShouldBeEquivalentTo(null),
                error => error.ShouldBeOfType<ConflictError>()
                    .Message.ShouldBe("Way of working already has a translation for language 'es'")
            );
    }

    [Fact]
    public void ReturnTheTranslationOfTheGivenLanguage()
    {
        var languageCode = LanguageCode.Create("en").IfLeft(error => throw new Exception(error.Message));

        _way.GetTranslation(languageCode)
            .Match(
                translation => translation.GetLanguageCode().ShouldBe("en"),
                error => error.ShouldBeNull()
            );
    }

    [Fact]
    public void FailWhenTheLanguageIsNotInTheWayOfWorking()
    {
        var languageCode = LanguageCode.Create("fr").IfLeft(error => throw new Exception(error.Message));

        _way.GetTranslation(languageCode)
            .Match(
                translation => translation.ShouldBeNull(),
                error => error.ShouldBeOfType<NotFoundError>()
                    .Message.ShouldBe("Way of working translation for language 'fr' was not found")
            );
    }
}
