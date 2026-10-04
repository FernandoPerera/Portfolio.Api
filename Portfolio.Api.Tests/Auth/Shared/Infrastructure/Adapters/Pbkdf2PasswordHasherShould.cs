using Portfolio.Api.Auth.Shared.Infrastructure.Adapters;
using Shouldly;

namespace Portfolio.Api.Tests.Auth.Shared.Infrastructure.Adapters;

public class Pbkdf2PasswordHasherShould
{
    [Fact]
    public void VerifyThePasswordThatWasHashed()
    {
        var hash = Pbkdf2PasswordHasher.Hash("secret");

        Pbkdf2PasswordHasher.Verify("secret", hash).ShouldBeTrue();
    }

    [Fact]
    public void RejectAnotherPassword()
    {
        var hash = Pbkdf2PasswordHasher.Hash("secret");

        Pbkdf2PasswordHasher.Verify("other", hash).ShouldBeFalse();
    }

    [Fact]
    public void GenerateADifferentHashEachTime()
    {
        Pbkdf2PasswordHasher.Hash("secret").ShouldNotBe(Pbkdf2PasswordHasher.Hash("secret"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-a-hash")]
    [InlineData("abc.def.ghi")]
    [InlineData("0.AAAA.AAAA")]
    public void RejectAMalformedHash(string malformedHash)
    {
        Pbkdf2PasswordHasher.Verify("secret", malformedHash).ShouldBeFalse();
    }
}
