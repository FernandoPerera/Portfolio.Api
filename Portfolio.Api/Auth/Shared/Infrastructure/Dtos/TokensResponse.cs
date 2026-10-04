using Portfolio.Api.Auth.Shared.Domain.Models;

namespace Portfolio.Api.Auth.Shared.Infrastructure.Dtos;

public record TokensResponse(string AccessToken, string RefreshToken)
{
    public static TokensResponse From(AuthTokens tokens) =>
        new(tokens.GetAccessToken(), tokens.GetRefreshToken());
}
