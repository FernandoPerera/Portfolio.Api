namespace Portfolio.Api.Auth.Login.Application.Dtos;

public record GitHubCallbackInfo(
    string Code,
    string State,
    string ExpectedState
);
