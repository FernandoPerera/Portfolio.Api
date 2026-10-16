using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;
using Shouldly;

namespace Portfolio.Api.Tests.Auth.Shared.Domain.Models;

public class RefreshTokenIdShould
{
    [Fact]
    public void BeCreated()
    {
        var given = Guid.NewGuid();

        RefreshTokenId.Create(given)
            .Match(
                created => created.GetValue().ShouldBe(given),
                error => error.ShouldBeNull()
            );
    }

    [Fact]
    public void NotAllowEmptyValue()
    {
        RefreshTokenId.Create(Guid.Empty)
            .Match(
                created => created.ShouldBeNull(),
                error => error.Exception.Case.ShouldBeOfType<ValidationException>()
                    .Message.ShouldBe("Refresh token id cannot be empty")
            );
    }
}
