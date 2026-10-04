using System.Security.Cryptography;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.AspNetCore.WebUtilities;
using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Auth.Shared.Domain.Ports;

namespace Portfolio.Api.Auth.Shared.Infrastructure.Adapters;

public class RandomRefreshTokenIssuer : IRefreshTokenIssuer
{
    private const int TokenSize = 64;

    public Either<Error, RefreshToken> Issue() =>
        RefreshToken.Create(WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(TokenSize)));
}
