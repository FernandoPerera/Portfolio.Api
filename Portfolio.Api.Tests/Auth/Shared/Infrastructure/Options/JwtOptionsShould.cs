using Portfolio.Api.Auth.Shared.Infrastructure.Options;
using Portfolio.Api.Tests.Shared.Extensions;
using Shouldly;

namespace Portfolio.Api.Tests.Auth.Shared.Infrastructure.Options;

public class JwtOptionsShould
{
    private const string ValidIssuer = "portfolio-api";
    private const string ValidAudience = "portfolio-clients";
    private const int MinimumSigningKeyLength = 32;
    private const int ValidMinutes = 15;
    private const int ValidDays = 7;

    [Fact]
    public void BeValid()
    {
        CreateOptions().GetInvalidMembers().ShouldBeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void RejectTheIssuerWhenItIsEmpty(string issuer)
    {
        CreateOptions(issuer: issuer).GetInvalidMembers().ShouldBe([nameof(JwtOptions.Issuer)]);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void RejectTheAudienceWhenItIsEmpty(string audience)
    {
        CreateOptions(audience: audience).GetInvalidMembers().ShouldBe([nameof(JwtOptions.Audience)]);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(MinimumSigningKeyLength - 1)]
    public void RejectTheSigningKeyWhenItIsTooShort(int length)
    {
        CreateOptions(signingKey: new string('k', length)).GetInvalidMembers().ShouldBe([nameof(JwtOptions.SigningKey)]);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void RejectTheAccessTokenLifetimeWhenItIsNotPositive(int minutes)
    {
        CreateOptions(accessTokenMinutes: minutes).GetInvalidMembers().ShouldBe([nameof(JwtOptions.AccessTokenMinutes)]);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void RejectTheRefreshTokenLifetimeWhenItIsNotPositive(int days)
    {
        CreateOptions(refreshTokenDays: days).GetInvalidMembers().ShouldBe([nameof(JwtOptions.RefreshTokenDays)]);
    }

    private static JwtOptions CreateOptions(
        string issuer = ValidIssuer,
        string audience = ValidAudience,
        string? signingKey = null,
        int accessTokenMinutes = ValidMinutes,
        int refreshTokenDays = ValidDays
    ) => new()
    {
        Issuer = issuer,
        Audience = audience,
        SigningKey = signingKey ?? new string('k', MinimumSigningKeyLength),
        AccessTokenMinutes = accessTokenMinutes,
        RefreshTokenDays = refreshTokenDays
    };
}
