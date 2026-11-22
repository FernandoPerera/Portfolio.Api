using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Tests.WaysOfWorking.Find.Domain.Builders;
using Portfolio.Api.Tests.WaysOfWorking.Shared.Domain.Builders;
using Portfolio.Api.WaysOfWorking.AddTranslation.Domain.Models;
using Shouldly;

namespace Portfolio.Api.Tests.WaysOfWorking.AddTranslation.Domain.Models;

public class WayTranslationToBeAddedShould
{
    [Fact]
    public void BeCreated()
    {
        var current = WayOfWorkingFoundBuilder.Build(translations: [WayTranslationBuilder.Build("es")]);
        var given = WayTranslationInputBuilder.Build("EN", "Clear", "No jargon");

        WayTranslationToBeAdded.Create(current, given)
            .Match(
                translation =>
                {
                    translation.GetWayId().ShouldBe(current.GetId());
                    translation.GetTranslation().GetLanguageCode().ShouldBe("en");
                    translation.GetTranslation().GetLabel().ShouldBe("Clear");
                    translation.GetTranslation().GetCaption().ShouldBe("No jargon");
                },
                error => error.ShouldBeNull()
            );
    }

    [Fact]
    public void RejectTranslationWhenTheWayAlreadyHasTheLanguage()
    {
        var current = WayOfWorkingFoundBuilder.Build(translations: [WayTranslationBuilder.Build("es")]);

        WayTranslationToBeAdded.Create(current, WayTranslationInputBuilder.Build("es"))
            .Match(
                translation => translation.ShouldBeNull(),
                error => error.ShouldBeOfType<ConflictError>()
                    .Message.ShouldBe("Way of working already has a translation for language 'es'")
            );
    }
}
