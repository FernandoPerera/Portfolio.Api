using LanguageExt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Languages.Update.Application.Dtos;
using Portfolio.Api.Languages.Update.Application.UseCases;
using Portfolio.Api.Languages.Update.Infrastructure.Dtos;
using Portfolio.Api.Shared.Infrastructure.Ports;

namespace Portfolio.Api.Languages.Update.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("v1/language")]
[Tags("Language")]
public class LanguageUpdaterController(
    LanguageUpdater updater,
    IErrorResponseMapper errorResponseMapper
) : ControllerBase
{
    /// <summary>
    /// Renames a language.
    /// </summary>
    /// <remarks>
    /// Changes the name of an existing language. Its code never changes. Only the owner can do it.
    /// </remarks>
    /// <param name="languageCode">Two-letter code of the language to rename, for example `en`.</param>
    /// <param name="request">The new name, up to 20 characters.</param>
    /// <response code="204">Language renamed.</response>
    /// <response code="400">The code does not have two letters, or the name is empty or longer than 20 characters.</response>
    /// <response code="401">The request has no valid access token. Sign in again or refresh the token.</response>
    /// <response code="403">The signed-in user is not the owner of the portfolio, so they cannot make changes.</response>
    /// <response code="404">There is no language with that code.</response>
    /// <response code="503">The language cannot be saved right now.</response>
    [HttpPatch("{languageCode}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult> Update(string languageCode, UpdateLanguageRequest request)
    {
        return await (
            from _ in updater.Execute(
                new LanguageUpdaterInfo(languageCode, request.Name)
            ).ToAsync()
            select Unit.Default
        ).Match(
            _ => NoContent(),
            error => errorResponseMapper.ToActionResult(this, error)
        );
    }
}
