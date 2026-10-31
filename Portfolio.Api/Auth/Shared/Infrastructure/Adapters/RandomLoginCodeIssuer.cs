using System.Security.Cryptography;
using LanguageExt;
using Microsoft.AspNetCore.WebUtilities;
using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Auth.Shared.Domain.Ports;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Auth.Shared.Infrastructure.Adapters;

public class RandomLoginCodeIssuer : ILoginCodeIssuer
{
    private const int CodeSize = 32;

    public Either<DomainError, LoginCode> Issue() =>
        LoginCode.Create(WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(CodeSize)));
}
