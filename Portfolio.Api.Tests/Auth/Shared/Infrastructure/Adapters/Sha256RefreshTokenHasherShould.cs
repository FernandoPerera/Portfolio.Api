using Portfolio.Api.Auth.Shared.Infrastructure.Adapters;
using Portfolio.Api.Tests.Auth.Shared.Domain.Builders;
using Shouldly;

namespace Portfolio.Api.Tests.Auth.Shared.Infrastructure.Adapters;

public class Sha256RefreshTokenHasherShould
{
    private readonly Sha256RefreshTokenHasher _hasher = new();

    [Fact]
    public void HashTheSameTokenToTheSameValue()
    {
        _hasher.Hash(RefreshTokenBuilder.Build("token"))
            .Match(
                first => _hasher.Hash(RefreshTokenBuilder.Build("token"))
                    .Match(
                        second => second.ToString().ShouldBe(first.ToString()),
                        error => error.ShouldBeNull()
                    ),
                error => error.ShouldBeNull()
            );
    }

    [Fact]
    public void HashDifferentTokensToDifferentValues()
    {
        _hasher.Hash(RefreshTokenBuilder.Build("token"))
            .Match(
                first => _hasher.Hash(RefreshTokenBuilder.Build("other-token"))
                    .Match(
                        second => second.ToString().ShouldNotBe(first.ToString()),
                        error => error.ShouldBeNull()
                    ),
                error => error.ShouldBeNull()
            );
    }
}
