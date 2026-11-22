using Portfolio.Api.Profiles.Shared.Domain.Models;
using Portfolio.Api.Tests.WaysOfWorking.Find.Domain.Builders;
using Portfolio.Api.Tests.WaysOfWorking.Shared.Domain.Builders;
using Shouldly;

namespace Portfolio.Api.Tests.WaysOfWorking.Find.Domain.Models;

public class WayOfWorkingFoundShould
{
    [Theory]
    [InlineData("es", true)]
    [InlineData("fr", false)]
    public void KnowIfItHasATranslationInALanguage(string givenLanguageCode, bool expected)
    {
        var way = WayOfWorkingFoundBuilder.Build(
            translations: [WayTranslationBuilder.Build("es"), WayTranslationBuilder.Build("en")]
        );
        var languageCode = LanguageCode.Create(givenLanguageCode).IfLeft(error => throw new Exception(error.Message));

        way.HasTranslationIn(languageCode).ShouldBe(expected);
    }
}
