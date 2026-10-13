using System.Security.Cryptography;
using System.Text;
using Portfolio.Api.Auth.Shared.Domain.Models;

namespace Portfolio.Api.Auth.Shared.Infrastructure.Adapters;

public static class LoginCodeHasher
{
    public static string Hash(LoginCode loginCode) =>
        Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(loginCode.ToString())));
}
