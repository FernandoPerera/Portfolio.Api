using Portfolio.Api.Tests.WaysOfWorking.Shared.Domain.Builders;
using Portfolio.Api.WaysOfWorking.Add.Domain.Models;
using Shouldly;

namespace Portfolio.Api.Tests.WaysOfWorking.AddTranslation.Domain.Models;

public class WayTranslationToBeAddedShould
{
    [Fact]
    public void BeCreated()
    {
        var givenId = Guid.NewGuid();
        var given = WayTranslationInputBuilder.Build("EN", "Clear", "No jargon");

        WayToBeAdded.Create(givenId, given)
            .Match(
                translation =>
                {
                    translation.GetWayId().ShouldBe(givenId);
                    translation.GetTranslation().GetLanguageCode().ShouldBe("en");
                    translation.GetTranslation().GetLabel().ShouldBe("Clear");
                    translation.GetTranslation().GetCaption().ShouldBe("No jargon");
                },
                error => error.ShouldBeNull()
            );
    }
}
