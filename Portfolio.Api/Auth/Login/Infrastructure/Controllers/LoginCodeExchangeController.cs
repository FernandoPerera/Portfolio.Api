using LanguageExt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Portfolio.Api.Auth.Login.Application.Dtos;
using Portfolio.Api.Auth.Login.Application.UseCases;
using Portfolio.Api.Auth.Login.Infrastructure.Dtos;
using Portfolio.Api.Auth.Shared.Infrastructure.Dtos;
using Portfolio.Api.Shared.Infrastructure.Extensions;
using Portfolio.Api.Shared.Infrastructure.Ports;

namespace Portfolio.Api.Auth.Login.Infrastructure.Controllers;

[ApiController]
[AllowAnonymous]
[EnableRateLimiting(RateLimitingExtensions.AuthPolicy)]
[Route("v1/auth/exchange")]
public class LoginCodeExchangeController(
    LoginCodeExchanger exchanger,
    IErrorResponseMapper errorResponseMapper
) : ControllerBase
{
    /// <summary>
    /// Swaps the login code for the session tokens.
    /// </summary>
    /// <remarks>
    /// Last step of the sign-in. The web sends the one-time login code it received and gets the access
    /// token and the refresh token. A login code works only once and expires after 60 seconds.
    /// </remarks>
    /// <param name="request">The login code received in the address after signing in with GitHub.</param>
    /// <response code="200">Tokens issued. Send the access token as `Authorization: Bearer` followed by the token.</response>
    /// <response code="400">The code is empty.</response>
    /// <response code="401">The code is unknown, was already used or has expired.</response>
    /// <response code="429">Too many requests from the same address. Wait for the number of seconds in the `Retry-After` header and try again.</response>
    /// <response code="503">The tokens cannot be issued right now.</response>
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType<TokensResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<TokensResponse>> Exchange(ExchangeRequest request)
    {
        return await (
            from tokens in exchanger.Execute(new ExchangeInfo(request.Code)).ToAsync()
            select TokensResponse.From(tokens)
        ).Match<ActionResult<TokensResponse>>(
            tokens => Ok(tokens),
            error => errorResponseMapper.ToActionResult(this, error)
        );
    }
}
