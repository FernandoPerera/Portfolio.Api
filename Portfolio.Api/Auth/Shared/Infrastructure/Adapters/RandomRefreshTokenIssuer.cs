using System.Security.Cryptography;
using LanguageExt;
using Microsoft.AspNetCore.WebUtilities;
using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Auth.Shared.Domain.Ports;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Auth.Shared.Infrastructure.Adapters;

public class RandomRefreshTokenIssuer : IRefreshTokenIssuer
{
    private const int TokenSize = 64;

    public Either<DomainError, RefreshToken> Issue() =>
        RefreshToken.Create(WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(TokenSize)));
}
