using LanguageExt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Portfolio.Api.Auth.Login.Application.Dtos;
using Portfolio.Api.Auth.Login.Application.UseCases;
using Portfolio.Api.Auth.Shared.Infrastructure.Options;
using Portfolio.Api.Auth.Login.Infrastructure.Cookies;
using Portfolio.Api.Shared.Infrastructure.Ports;

namespace Portfolio.Api.Auth.Login.Infrastructure.Controllers;

[ApiController]
[AllowAnonymous]
[Route("v1/auth/github")]
public class GitHubCallbackController(
    GitHubLoginCompleter completer,
    AuthOptions authOptions,
    IErrorResponseMapper errorResponseMapper
) : ControllerBase
{
    [HttpGet("callback")]
    public async Task<ActionResult> Callback([FromQuery] string? code, [FromQuery] string? state)
    {
        var expectedState = Request.Cookies[OAuthStateCookie.Name] ?? string.Empty;
        Response.Cookies.Delete(OAuthStateCookie.Name, new CookieOptions { Path = OAuthStateCookie.Path });

        return await (
            from loginCode in completer.Execute(new GitHubCallbackInfo(code ?? string.Empty, state ?? string.Empty, expectedState)).ToAsync()
            select QueryHelpers.AddQueryString(authOptions.FrontendRedirectUri, "code", loginCode.ToString())
        ).Match<ActionResult>(
            redirectUrl => Redirect(redirectUrl),
            error => errorResponseMapper.ToActionResult(this, error)
        );
    }
}
