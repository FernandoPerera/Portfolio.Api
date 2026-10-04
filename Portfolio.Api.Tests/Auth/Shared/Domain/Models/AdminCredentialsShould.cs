using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;
using Shouldly;

namespace Portfolio.Api.Tests.Auth.Shared.Domain.Models;

public class AdminCredentialsShould
{
    [Fact]
    public void BeCreated()
    {
        AdminCredentials.Create("admin", "secret")
            .Match(
                credentials =>
                {
                    credentials.GetUsername().ToString().ShouldBe("admin");
                    credentials.GetPassword().ShouldBe("secret");
                },
                error => error.ShouldBeNull()
            );
    }

    [Theory]
    [InlineData("", "secret", "Username cannot be empty")]
    [InlineData("admin", "", "Password cannot be empty")]
    public void RejectCredentialsWhenAPartIsMissing(string username, string password, string expectedMessage)
    {
        AdminCredentials.Create(username, password)
            .Match(
                credentials => credentials.ShouldBeNull(),
                error => error.Exception.Case.ShouldBeOfType<ValidationException>()
                    .Message.ShouldBe(expectedMessage)
            );
    }
}
