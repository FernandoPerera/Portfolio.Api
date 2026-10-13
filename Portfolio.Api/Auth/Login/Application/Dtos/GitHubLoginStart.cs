using Portfolio.Api.Auth.Shared.Domain.Models;

namespace Portfolio.Api.Auth.Login.Application.Dtos;

public record GitHubLoginStart(
    OAuthState State,
    Uri AuthorizationUrl
);
