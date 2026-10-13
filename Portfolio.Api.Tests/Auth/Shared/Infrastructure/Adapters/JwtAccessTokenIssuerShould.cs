using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Portfolio.Api.Auth.Shared.Infrastructure.Adapters;
using Portfolio.Api.Auth.Shared.Infrastructure.Authorization;
using Portfolio.Api.Auth.Shared.Infrastructure.Options;
using Portfolio.Api.Tests.Auth.Shared.Domain.Builders;
using Portfolio.Api.Tests.Shared.Infrastructure;
using Shouldly;

namespace Portfolio.Api.Tests.Auth.Shared.Infrastructure.Adapters;

public class JwtAccessTokenIssuerShould
{
    private static readonly DateTimeOffset Now = new(2026, 1, 1, 10, 0, 0, TimeSpan.Zero);

    private readonly JwtOptions _options = new()
    {
        Issuer = "portfolio-api",
        Audience = "portfolio-clients",
        SigningKey = "a-signing-key-with-at-least-32-bytes!",
        AccessTokenMinutes = 15
    };

    [Fact]
    public async Task IssueASignedTokenForTheAdmin()
    {
        var issuer = new JwtAccessTokenIssuer(_options, new FixedTimeProvider(Now));

        var result = issuer.Issue(AdminUsernameBuilder.Build("admin"));

        var token = result.IfLeft(error => throw new Exception(error.Message)).ToString();
        var validation = await new JsonWebTokenHandler().ValidateTokenAsync(token, new TokenValidationParameters
        {
            ValidIssuer = _options.Issuer,
            ValidAudience = _options.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey)),
            ValidateLifetime = false
        });
        validation.IsValid.ShouldBeTrue();
        var jwt = new JsonWebTokenHandler().ReadJsonWebToken(token);
        jwt.Subject.ShouldBe("admin");
        jwt.GetClaim(AdminAccess.RoleClaimType).Value.ShouldBe(AdminAccess.RoleClaimValue);
        jwt.ValidTo.ShouldBe(Now.UtcDateTime.AddMinutes(15));
    }
}
