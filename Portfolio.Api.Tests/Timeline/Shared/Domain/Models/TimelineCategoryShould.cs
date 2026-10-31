using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Timeline.Shared.Domain.Models;
using Shouldly;

namespace Portfolio.Api.Tests.Timeline.Shared.Domain.Models;

public class TimelineCategoryShould
{
    [Theory]
    [InlineData("work", "work")]
    [InlineData("education", "education")]
    [InlineData("  WORK ", "work")]
    [InlineData("Education", "education")]
    public void BeCreated(string givenCategory, string expectedCategory)
    {
        TimelineCategory.Create(givenCategory)
            .Match(
                category => category.ToString().ShouldBe(expectedCategory),
                error => error.ShouldBeNull()
            );
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void NotAllowEmptyValue(string givenCategory)
    {
        TimelineCategory.Create(givenCategory)
            .Match(
                category => category.ShouldBeNull(),
                error => error.ShouldBeOfType<ValidationError>()
                    .Message.ShouldBe("Category cannot be empty")
            );
    }

    [Theory]
    [InlineData("hobby")]
    [InlineData("works")]
    public void RejectCategoryWhenItIsNotSupported(string givenCategory)
    {
        TimelineCategory.Create(givenCategory)
            .Match(
                category => category.ShouldBeNull(),
                error => error.ShouldBeOfType<ValidationError>()
                    .Message.ShouldBe("Category must be 'work' or 'education'")
            );
    }
}
