using Portfolio.Api.WaysOfWorking.Update.Domain.Models;
using Shouldly;

namespace Portfolio.Api.Tests.WaysOfWorking.Update.Domain.Models;

public class WayOfWorkingToBeUpdatedShould
{
    [Fact]
    public void BeCreated()
    {
        var givenId = Guid.NewGuid();
        const string givenIllustration = "fast-response";

        WayOfWorkingToBeUpdated.Create(givenId, givenIllustration)
            .Match(
                way =>
                {
                    way.GetId().ShouldBe(givenId);
                    way.GetIllustration().ShouldBe(givenIllustration);
                },
                error => error.ShouldBeNull()
            );
    }
}
