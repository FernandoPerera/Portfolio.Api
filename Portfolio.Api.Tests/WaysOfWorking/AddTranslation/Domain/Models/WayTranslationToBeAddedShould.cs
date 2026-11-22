using Portfolio.Api.Tests.WaysOfWorking.Shared.Domain.Builders;
using Portfolio.Api.WaysOfWorking.AddTranslation.Domain.Models;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Models;
using Shouldly;

namespace Portfolio.Api.Tests.WaysOfWorking.AddTranslation.Domain.Models;

public class WayTranslationToBeAddedShould
{
    [Fact]
    public void BeCreated()
    {
        var givenId = Guid.NewGuid();
        var wayId = WayOfWorkingId.Create(givenId).IfLeft(error => throw new Exception(error.Message));
        var given = WayTranslationInputBuilder.Build("EN", "Clear", "No jargon");

        WayTranslationToBeAdded.Create(wayId, given)
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
