using LanguageExt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Auth.Login.Application.Dtos;
using Portfolio.Api.Auth.Login.Application.UseCases;
using Portfolio.Api.Auth.Login.Infrastructure.Dtos;
using Portfolio.Api.Auth.Shared.Infrastructure.Dtos;
using Portfolio.Api.Shared.Infrastructure.Ports;

namespace Portfolio.Api.Auth.Login.Infrastructure.Controllers;

[ApiController]
[AllowAnonymous]
[Route("v1/auth/login")]
public class AuthLoginController(
    Authenticator authenticator,
    IErrorResponseMapper errorResponseMapper
) : ControllerBase
{
    [HttpPost]
    [Produces("application/json")]
    public async Task<ActionResult<TokensResponse>> Login(LoginRequest request)
    {
        return await (
            from tokens in authenticator.Execute(new LoginInfo(request.Username, request.Password)).ToAsync()
            select TokensResponse.From(tokens)
        ).Match<ActionResult<TokensResponse>>(
            tokens => Ok(tokens),
            error => errorResponseMapper.ToActionResult(this, error)
        );
    }
}
