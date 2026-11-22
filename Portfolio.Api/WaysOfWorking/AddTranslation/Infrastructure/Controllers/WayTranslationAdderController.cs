using LanguageExt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Shared.Infrastructure.Ports;
using Portfolio.Api.WaysOfWorking.AddTranslation.Application.Dtos;
using Portfolio.Api.WaysOfWorking.AddTranslation.Application.UseCases;
using Portfolio.Api.WaysOfWorking.AddTranslation.Infrastructure.Dtos;

namespace Portfolio.Api.WaysOfWorking.AddTranslation.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("v1/ways-of-working")]
[Tags("Ways of working")]
public class WayTranslationAdderController(
    WayTranslationAdder adder,
    IErrorResponseMapper errorResponseMapper
) : ControllerBase
{
    /// <summary>
    /// Adds a translation to a way of working.
    /// </summary>
    /// <remarks>
    /// Writes the label, caption, title, description and points of an existing way of working in one more
    /// language. The language must exist and the way of working must not have a translation in it yet.
    /// Only the owner can do it.
    /// </remarks>
    /// <param name="id">Identifier of the way of working.</param>
    /// <param name="languageCode">Two-letter code of the language of the new translation, for example `en`.</param>
    /// <param name="request">The label (up to 30 characters), caption (up to 60), title (up to 100), description (up to 500) and between 1 and 5 different points (up to 120 characters each) in that language.</param>
    /// <response code="201">Translation added.</response>
    /// <response code="400">A text is empty or too long, the points are not valid, or the language code is not valid.</response>
    /// <response code="401">The request has no valid access token. Sign in again or refresh the token.</response>
    /// <response code="403">The signed-in user is not the owner of the portfolio, so they cannot make changes.</response>
    /// <response code="404">The way of working or the language does not exist.</response>
    /// <response code="409">The way of working already has a translation in that language.</response>
    /// <response code="503">The translation cannot be saved right now.</response>
    [HttpPost("{id:guid}/{languageCode}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult> Add(Guid id, string languageCode, AddWayTranslationRequest request)
    {
        return await (
            from _ in adder.Execute(
                new WayTranslationAdderInfo(
                    id,
                    languageCode,
                    request.Label,
                    request.Caption,
                    request.Title,
                    request.Description,
                    request.Points
                )
            ).ToAsync()
            select Unit.Default
        ).Match(
            _ => CreatedAtAction(nameof(Add), request),
            error => errorResponseMapper.ToActionResult(this, error)
        );
    }
}
