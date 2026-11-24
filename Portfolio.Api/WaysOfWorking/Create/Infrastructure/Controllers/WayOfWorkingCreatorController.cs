using LanguageExt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Shared.Infrastructure.Ports;
using Portfolio.Api.WaysOfWorking.Create.Application.Dtos;
using Portfolio.Api.WaysOfWorking.Create.Application.UseCases;
using Portfolio.Api.WaysOfWorking.Create.Infrastructure.Dtos;

namespace Portfolio.Api.WaysOfWorking.Create.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("v1/ways-of-working")]
[Tags("Ways of working")]
public class WayOfWorkingCreatorController(
    WayOfWorkingCreator creator,
    IErrorResponseMapper errorResponseMapper
) : ControllerBase
{
    /// <summary>
    /// Adds a way of working.
    /// </summary>
    /// <remarks>
    /// Creates a way of working with at least one translation. It goes last in the list; the order is
    /// changed with the order endpoint. The illustration is a key written in lowercase letters, digits and
    /// hyphens between words (up to 30 characters). Each translation has a label (up to 30 characters), a
    /// caption (up to 60), a title (up to 100), a description (up to 500) and between 1 and 5 different
    /// points (up to 120 characters each). Every language must exist and cannot be used twice in the same
    /// way of working. Only the owner can do it.
    /// </remarks>
    /// <param name="request">The illustration and the translations of the way of working.</param>
    /// <response code="201">Way of working created.</response>
    /// <response code="400">A required field is empty, a text is too long, the illustration or the points are not valid, or a language is repeated.</response>
    /// <response code="401">The request has no valid access token. Sign in again or refresh the token.</response>
    /// <response code="403">The signed-in user is not the owner of the portfolio, so they cannot make changes.</response>
    /// <response code="404">One of the languages does not exist. Create it first.</response>
    /// <response code="503">The way of working cannot be saved right now.</response>
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult> Create(CreateWayOfWorkingRequest request)
    {
        return await (
            from _ in creator.Execute(
                new WayOfWorkingCreatorInfo(
                    request.Illustration,
                    request.Translations
                        .Select(translation => new WayTranslationInfo(
                            translation.LanguageCode,
                            translation.Label,
                            translation.Caption,
                            translation.Title,
                            translation.Description,
                            translation.Points
                        ))
                        .ToList()
                )
            ).ToAsync()
            select Unit.Default
        ).Match(
            _ => CreatedAtAction(nameof(Create), request),
            error => errorResponseMapper.ToActionResult(this, error)
        );
    }
}
