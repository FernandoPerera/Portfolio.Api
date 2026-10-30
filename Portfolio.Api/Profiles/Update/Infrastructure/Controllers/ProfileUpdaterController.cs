using LanguageExt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Profiles.Update.Application.Dtos;
using Portfolio.Api.Profiles.Update.Application.UseCases;
using Portfolio.Api.Profiles.Update.Infrastructure.Dtos;
using Portfolio.Api.Shared.Infrastructure.Ports;

namespace Portfolio.Api.Profiles.Update.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("v1/profile")]
[Tags("Profile")]
public class ProfileUpdaterController(
    ProfileUpdater updater,
    IErrorResponseMapper errorResponseMapper
) : ControllerBase
{
    /// <summary>
    /// Edits the profile in one language.
    /// </summary>
    /// <remarks>
    /// Changes only the fields that are sent. The name and the company are shared by all languages, while
    /// the job role and the presentation belong to the language in the address. Only the owner can do it.
    /// </remarks>
    /// <param name="languageCode">Two-letter code of the language whose translation is edited, for example `es`.</param>
    /// <param name="request">The fields to change. The ones left out stay as they are.</param>
    /// <response code="204">Profile updated.</response>
    /// <response code="400">A sent field is empty or too long.</response>
    /// <response code="401">The request has no valid access token. Sign in again or refresh the token.</response>
    /// <response code="403">The signed-in user is not the owner of the portfolio, so they cannot make changes.</response>
    /// <response code="404">The profile does not exist, or it has no translation in that language.</response>
    /// <response code="503">The profile cannot be saved right now.</response>
    [HttpPatch("{languageCode}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult> Update(string languageCode, UpdateProfileRequest request)
    {
        return await (
            from _ in updater.Execute(
                new ProfileUpdaterInfo(
                    request.Name,
                    request.EnterpriseName,
                    request.JobRole,
                    request.Presentation,
                    languageCode
                )
            ).ToAsync()
            select Unit.Default
        ).Match(
            _ => NoContent(),
            error => errorResponseMapper.ToActionResult(this, error)
        );
    }
}
