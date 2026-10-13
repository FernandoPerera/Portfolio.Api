using System.Security.Cryptography;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.AspNetCore.WebUtilities;
using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Auth.Shared.Domain.Ports;

namespace Portfolio.Api.Auth.Shared.Infrastructure.Adapters;

public class RandomOAuthStateIssuer : IOAuthStateIssuer
{
    private const int StateSize = 32;

    public Either<Error, OAuthState> Issue() =>
        OAuthState.Create(WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(StateSize)));
}
