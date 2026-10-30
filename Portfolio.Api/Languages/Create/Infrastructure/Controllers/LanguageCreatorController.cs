using LanguageExt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Languages.Create.Application.Dtos;
using Portfolio.Api.Languages.Create.Application.UseCases;
using Portfolio.Api.Languages.Create.Infrastructure.Dtos;
using Portfolio.Api.Shared.Infrastructure.Ports;

namespace Portfolio.Api.Languages.Create.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("v1/language")]
public class LanguageCreatorController(
    LanguageCreator creator,
    IErrorResponseMapper errorResponseMapper
) : ControllerBase
{
    /// <summary>
    /// Adds a language.
    /// </summary>
    /// <remarks>
    /// Registers a new language, with a two-letter code and a name of up to 20 characters, so the profile
    /// and the timeline can be translated into it. Only the owner can do it.
    /// </remarks>
    /// <param name="request">The two-letter code of the language, for example `en`, and its name, for example `English`.</param>
    /// <response code="201">Language created.</response>
    /// <response code="400">The code does not have two letters, or the name is empty or longer than 20 characters.</response>
    /// <response code="401">The request has no valid access token. Sign in again or refresh the token.</response>
    /// <response code="403">The signed-in user is not the owner of the portfolio, so they cannot make changes.</response>
    /// <response code="503">The language cannot be saved right now.</response>
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult> Create(CreateLanguageRequest request)
    {
        return await (
            from _ in creator.Execute(
                new LanguageCreatorInfo(request.LanguageCode, request.Name)
            ).ToAsync()
            select Unit.Default
        ).Match(
            _ => CreatedAtAction(nameof(Create), request),
            error => errorResponseMapper.ToActionResult(this, error)
        );
    }
}
