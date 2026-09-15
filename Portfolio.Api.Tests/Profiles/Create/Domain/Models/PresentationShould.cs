using Portfolio.Api.Profiles.Create.Domain.Models;
using Shouldly;

namespace Portfolio.Api.Tests.Profiles.Create.Domain.Models;

public class PresentationShould
{
    [Fact]
    public void BeCreated()
    {
        const string givenPresentation = "Mucho gusto :)";

        Presentation.Create(givenPresentation).Match(
            presentation => presentation.ToString().ShouldBe(givenPresentation),
            error => error.ShouldBeNull()
        );
    }
}