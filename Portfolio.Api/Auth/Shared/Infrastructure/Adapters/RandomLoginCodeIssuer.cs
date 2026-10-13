using System.Security.Cryptography;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.AspNetCore.WebUtilities;
using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Auth.Shared.Domain.Ports;

namespace Portfolio.Api.Auth.Shared.Infrastructure.Adapters;

public class RandomLoginCodeIssuer : ILoginCodeIssuer
{
    private const int CodeSize = 32;

    public Either<Error, LoginCode> Issue() =>
        LoginCode.Create(WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(CodeSize)));
}
