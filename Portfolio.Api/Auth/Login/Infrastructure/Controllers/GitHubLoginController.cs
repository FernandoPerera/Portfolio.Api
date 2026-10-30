using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Portfolio.Api.Auth.Login.Application.UseCases;
using Portfolio.Api.Auth.Login.Infrastructure.Cookies;
using Portfolio.Api.Shared.Infrastructure.Extensions;
using Portfolio.Api.Shared.Infrastructure.Ports;

namespace Portfolio.Api.Auth.Login.Infrastructure.Controllers;

[ApiController]
[AllowAnonymous]
[EnableRateLimiting(RateLimitingExtensions.AuthPolicy)]
[Route("v1/auth/github")]
[Tags("Auth")]
public class GitHubLoginController(
    GitHubLoginStarter starter,
    IErrorResponseMapper errorResponseMapper
) : ControllerBase
{
    /// <summary>
    /// Starts the owner's sign-in with GitHub.
    /// </summary>
    /// <remarks>
    /// First step of the sign-in. Open this address in the browser: the API sends the user to GitHub to
    /// sign in and accept the access. Nothing is returned in the body.
    /// </remarks>
    /// <response code="302">The browser is sent to GitHub to sign in and accept the access.</response>
    /// <response code="429">Too many requests from the same address. Wait for the number of seconds in the `Retry-After` header and try again.</response>
    [HttpGet("login")]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status429TooManyRequests)]
    public ActionResult Login()
    {
        return starter.Execute().Match<ActionResult>(
            start =>
            {
                Response.Cookies.Append(OAuthStateCookie.Name, start.State.ToString(), new CookieOptions
                {
                    HttpOnly = true,
                    Secure = Request.IsHttps,
                    SameSite = SameSiteMode.Lax,
                    MaxAge = OAuthStateCookie.Lifetime,
                    Path = OAuthStateCookie.Path
                });

                return Redirect(start.AuthorizationUrl.ToString());
            },
            error => errorResponseMapper.ToActionResult(this, error)
        );
    }
}
