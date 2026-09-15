using Portfolio.Api.Profiles.Create.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;
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

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void NotAllowEmptyValue(string givenPresentation)
    {
        Presentation.Create(givenPresentation).Match(
            presentation => presentation.ShouldBeNull(),
            error => error.Exception.Case.ShouldBeOfType<ValidationException>()
                .Message.ShouldBe("Presentation cannot be empty")
        );
    }
}