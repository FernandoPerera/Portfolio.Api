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
public class GitHubLoginController(
    GitHubLoginStarter starter,
    IErrorResponseMapper errorResponseMapper
) : ControllerBase
{
    [HttpGet("login")]
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
