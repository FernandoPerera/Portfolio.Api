using Portfolio.Api.Profiles.Create.Domain.Models;
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
    
}