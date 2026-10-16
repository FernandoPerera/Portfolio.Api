using System.Security.Cryptography;
using System.Text;
using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Auth.Shared.Domain.Ports;

namespace Portfolio.Api.Auth.Shared.Infrastructure.Adapters;

public class Sha256RefreshTokenHasher : IRefreshTokenHasher
{
    public Either<Error, RefreshTokenHash> Hash(RefreshToken refreshToken) =>
        RefreshTokenHash.Create(
            Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken.ToString())))
        );
}
