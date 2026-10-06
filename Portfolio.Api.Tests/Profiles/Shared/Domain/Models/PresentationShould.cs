using Portfolio.Api.Profiles.Shared.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;
using Shouldly;

namespace Portfolio.Api.Tests.Profiles.Shared.Domain.Models;

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

    [Fact]
    public void DoNotCreateIfTheCharacterLimitIsExceeded()
    {
        var givenPresentation = new string('a', 751);
        
        Presentation.Create(givenPresentation).Match(
            presentation => presentation.ShouldBeNull(),
            error => error.Exception.Case.ShouldBeOfType<ValidationException>()
                .Message.ShouldBe("The presentation cannot exceed 750 characters")
        );
    }
}