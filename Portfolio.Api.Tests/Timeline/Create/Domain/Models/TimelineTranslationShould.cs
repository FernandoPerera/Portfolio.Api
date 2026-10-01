using Portfolio.Api.Timeline.Create.Domain.Models;
using Shouldly;

namespace Portfolio.Api.Tests.Timeline.Create.Domain.Models;

public class TimelineTranslationShould
{
    [Fact]
    public void BeCreated()
    {
        const string givenLanguageCode = "es";
        const string givenTitle = "Frontend Lead";
        const string givenDescription = "Liderazgo técnico en la migración a arquitecturas Jamstack.";

        TimelineTranslation.Create(givenLanguageCode, givenTitle, givenDescription)
            .Match(
                translation =>
                {
                    translation.GetLanguageCode().ShouldBe(givenLanguageCode);
                    translation.GetTitle().ShouldBe(givenTitle);
                    translation.GetDescription().ShouldBe(givenDescription);
                },
                error => error.ShouldBeNull()
            );
    }
}
