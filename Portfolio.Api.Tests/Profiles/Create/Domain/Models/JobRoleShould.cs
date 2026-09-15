using Portfolio.Api.Profiles.Create.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;
using Shouldly;

namespace Portfolio.Api.Tests.Profiles.Create.Domain.Models;

public class JobRoleShould
{
    
    [Fact]
    public void BeCreated()
    {
        const string givenJobRole = "Beer software architect";

        JobRole.Create(givenJobRole)
            .Match(
                name => name.ToString().ShouldBe(givenJobRole),
                error => error.ShouldBeNull()
            );
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void NotAllowEmptyValue(string givenJobRole)
    {
        JobRole.Create(givenJobRole)
            .Match(
                name => name.ShouldBeNull(),
                error => error.Exception.Case.ShouldBeOfType<ValidationException>()
                    .Message.ShouldBe("Job role cannot be empty")
            );
    }
    
}