using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Shared.Infrastructure.Ports;
using Portfolio.Api.WaysOfWorking.UpdateTranslation.Application.Dtos;
using Portfolio.Api.WaysOfWorking.UpdateTranslation.Application.UseCases;
using Portfolio.Api.WaysOfWorking.UpdateTranslation.Infrastructure.Dtos;

namespace Portfolio.Api.WaysOfWorking.UpdateTranslation.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("v1/ways-of-working")]
[Tags("Ways of working")]
public class WayTranslationUpdaterController(
    WayTranslationUpdater updater,
    IErrorResponseMapper errorResponseMapper
) : ControllerBase
{
    /// <summary>
    /// Edits a way of working in one language.
    /// </summary>
    /// <remarks>
    /// Changes only the fields that are sent: the label, caption, title, description and points of the
    /// translation in the language of the address. The way of working must already have a translation in
    /// that language; add it first if it does not. Only the owner can do it.
    /// </remarks>
    /// <param name="id">Identifier of the way of working.</param>
    /// <param name="languageCode">Two-letter code of the language whose texts are edited, for example `es`.</param>
    /// <param name="request">The fields to change. The ones left out stay as they are.</param>
    /// <response code="204">Translation updated.</response>
    /// <response code="400">A sent field is empty or too long, or the points are not valid.</response>
    /// <response code="401">The request has no valid access token. Sign in again or refresh the token.</response>
    /// <response code="403">The signed-in user is not the owner of the portfolio, so they cannot make changes.</response>
    /// <response code="404">The way of working does not exist, or it has no translation in that language.</response>
    /// <response code="503">The translation cannot be saved right now.</response>
    [HttpPatch("{id:guid}/{languageCode}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult> Update(Guid id, string languageCode, UpdateWayTranslationRequest request)
    {
        return await updater.Execute(
                new WayTranslationUpdaterInfo(
                    id,
                    languageCode,
                    request.Label,
                    request.Caption,
                    request.Title,
                    request.Description,
                    request.Points
                )
            )
            .ToAsync()
            .Match(
                _ => NoContent(),
                error => errorResponseMapper.ToActionResult(this, error)
            );
    }
}
