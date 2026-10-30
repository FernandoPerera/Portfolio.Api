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
    [HttpPost]
    [Produces("application/json")]
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
