using LanguageExt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Auth.Refresh.Application.Dtos;
using Portfolio.Api.Auth.Refresh.Application.UseCases;
using Portfolio.Api.Auth.Refresh.Infrastructure.Dtos;
using Portfolio.Api.Auth.Shared.Infrastructure.Dtos;
using Portfolio.Api.Shared.Infrastructure.Ports;

namespace Portfolio.Api.Auth.Refresh.Infrastructure.Controllers;

[ApiController]
[AllowAnonymous]
[Route("v1/auth/refresh")]
public class AuthRefreshController(
    TokenRefresher refresher,
    IErrorResponseMapper errorResponseMapper
) : ControllerBase
{
    [HttpPost]
    [Produces("application/json")]
    public async Task<ActionResult<TokensResponse>> Refresh(RefreshRequest request)
    {
        return await (
            from tokens in refresher.Execute(new RefreshInfo(request.RefreshToken)).ToAsync()
            select TokensResponse.From(tokens)
        ).Match<ActionResult<TokensResponse>>(
            tokens => Ok(tokens),
            error => errorResponseMapper.ToActionResult(this, error)
        );
    }
}
