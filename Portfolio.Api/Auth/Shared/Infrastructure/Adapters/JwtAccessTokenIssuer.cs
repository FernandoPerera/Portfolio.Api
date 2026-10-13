using System.Security.Claims;
using System.Text;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Auth.Shared.Domain.Ports;
using Portfolio.Api.Auth.Shared.Infrastructure.Authorization;
using Portfolio.Api.Auth.Shared.Infrastructure.Options;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Auth.Shared.Infrastructure.Adapters;

public class JwtAccessTokenIssuer(JwtOptions options, TimeProvider timeProvider) : IAccessTokenIssuer
{
    public Either<Error, AccessToken> Issue(AdminUsername username)
    {
        try
        {
            var now = timeProvider.GetUtcNow().UtcDateTime;
            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = options.Issuer,
                Audience = options.Audience,
                Subject = new ClaimsIdentity([
                    new Claim(JwtRegisteredClaimNames.Sub, username.ToString()),
                    new Claim(AdminAccess.RoleClaimType, AdminAccess.RoleClaimValue)
                ]),
                IssuedAt = now,
                NotBefore = now,
                Expires = now.AddMinutes(options.AccessTokenMinutes),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SigningKey)),
                    SecurityAlgorithms.HmacSha256
                )
            };

            var token = new JsonWebTokenHandler().CreateToken(descriptor);

            return AccessToken.Create(token);
        }
        catch (Exception error)
        {
            return Error.New(new ServiceUnavailableException("Access token cannot be issued, error: " + error.Message));
        }
    }
}
