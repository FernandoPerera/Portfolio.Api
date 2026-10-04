using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Timeline.Shared.Domain.Models;
using Shouldly;

namespace Portfolio.Api.Tests.Timeline.Shared.Domain.Models;

public class TechnologiesShould
{
    [Fact]
    public void BeCreated()
    {
        var givenTechnologies = new List<string> { "React", "TypeScript" };

        Technologies.Create(givenTechnologies)
            .Match(
                technologies => technologies.GetValues().ShouldBe(givenTechnologies),
                error => error.ShouldBeNull()
            );
    }

    [Fact]
    public void BeCreatedWithoutTechnologies()
    {
        Technologies.Create([])
            .Match(
                technologies => technologies.GetValues().ShouldBeEmpty(),
                error => error.ShouldBeNull()
            );
    }

    [Theory]
    [InlineData("React", "React")]
    [InlineData("React", "react")]
    [InlineData("React", "  REACT ")]
    public void RejectTechnologiesWhenOneIsRepeated(string first, string second)
    {
        Technologies.Create([first, second])
            .Match(
                technologies => technologies.ShouldBeNull(),
                error => error.Exception.Case.ShouldBeOfType<ValidationException>()
                    .Message.ShouldBe("Technologies cannot be repeated")
            );
    }
}
