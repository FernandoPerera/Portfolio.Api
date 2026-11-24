using Portfolio.Api.Tests.WaysOfWorking.Shared.Domain.Builders;
using Portfolio.Api.WaysOfWorking.Create.Domain.Models;
using Shouldly;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Inputs;

namespace Portfolio.Api.Tests.WaysOfWorking.Create.Domain.Models;

public class WayOfWorkingToBeCreatedShould
{
    [Fact]
    public void BeCreated()
    {
        const string givenIllustration = "tested";
        var spanish = WayTranslationInputBuilder.Build("es", "Probado");
        var english = WayTranslationInputBuilder.Build("en", "Tested");

        WayOfWorkingToBeCreated.Create(givenIllustration, [spanish, english])
            .Match(
                way =>
                {
                    way.GetIllustration().ShouldBe(givenIllustration);
                    way.GetTranslations().Select(translation => translation.GetLabel()).ShouldBe(["Probado", "Tested"]);
                },
                error => error.ShouldBeNull()
            );
    }
}
