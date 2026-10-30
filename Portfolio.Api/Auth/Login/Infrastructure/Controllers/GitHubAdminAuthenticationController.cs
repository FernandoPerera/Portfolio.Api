using LanguageExt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.WebUtilities;
using Portfolio.Api.Auth.Login.Application.Dtos;
using Portfolio.Api.Auth.Login.Application.UseCases;
using Portfolio.Api.Auth.Shared.Infrastructure.Options;
using Portfolio.Api.Auth.Login.Infrastructure.Cookies;
using Portfolio.Api.Shared.Infrastructure.Extensions;
using Portfolio.Api.Shared.Infrastructure.Ports;

namespace Portfolio.Api.Auth.Login.Infrastructure.Controllers;

[ApiController]
[AllowAnonymous]
[EnableRateLimiting(RateLimitingExtensions.AuthPolicy)]
[Route("v1/auth/github")]
[Tags("Auth")]
public class GitHubAdminAuthenticationController(
    GitHubAdminAuthenticator authenticator,
    AuthOptions authOptions,
    IErrorResponseMapper errorResponseMapper
) : ControllerBase
{
    /// <summary>
    /// Finishes the sign-in after GitHub has answered.
    /// </summary>
    /// <remarks>
    /// GitHub sends the user back here after accepting. The API checks that the sign-in was started by this
    /// browser, asks GitHub who the user is and verifies that they are the owner. If so, the user goes back
    /// to the web with a one-time login code in the address (`?code=...`) that must be exchanged for the
    /// tokens. This address is followed by the browser, it is not called by hand.
    /// </remarks>
    /// <param name="code">Temporary code that GitHub gives after the user accepts.</param>
    /// <param name="state">Random value that ties this answer to the sign-in started in the same browser.</param>
    /// <response code="302">The user is the owner. The browser goes back to the web with a one-time login code in the address.</response>
    /// <response code="400">The code or the state is missing.</response>
    /// <response code="401">The sign-in was not started by this browser, or GitHub rejected the code.</response>
    /// <response code="403">The GitHub user is not the owner of the portfolio.</response>
    /// <response code="429">Too many requests from the same address. Wait for the number of seconds in the `Retry-After` header and try again.</response>
    /// <response code="503">GitHub or the database cannot be reached right now.</response>
    [HttpGet("callback")]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult> Callback([FromQuery] string? code, [FromQuery] string? state)
    {
        var expectedState = Request.Cookies[OAuthStateCookie.Name] ?? string.Empty;
        Response.Cookies.Delete(OAuthStateCookie.Name, new CookieOptions { Path = OAuthStateCookie.Path });

        return await (
            from loginCode in authenticator.Execute(new GitHubCallbackInfo(code ?? string.Empty, state ?? string.Empty, expectedState)).ToAsync()
            select QueryHelpers.AddQueryString(authOptions.FrontendRedirectUri, "code", loginCode.ToString())
        ).Match<ActionResult>(
            redirectUrl => Redirect(redirectUrl),
            error => errorResponseMapper.ToActionResult(this, error)
        );
    }
}
