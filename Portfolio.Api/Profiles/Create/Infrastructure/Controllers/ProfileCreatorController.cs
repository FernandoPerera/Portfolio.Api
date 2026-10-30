using LanguageExt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Profiles.Create.Application.Dtos;
using Portfolio.Api.Profiles.Create.Application.UseCases;
using Portfolio.Api.Profiles.Create.Infrastructure.Dtos;
using Portfolio.Api.Shared.Infrastructure.Ports;

namespace Portfolio.Api.Profiles.Create.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("v1/profile")]
[Tags("Profile")]
public class ProfileCreatorController(
    ProfileCreator creator,
    IErrorResponseMapper errorResponseMapper
) : ControllerBase
{
    /// <summary>
    /// Creates the profile of the portfolio owner.
    /// </summary>
    /// <remarks>
    /// Creates the profile with its first translation. The name (up to 100 characters), the job role and
    /// the presentation (up to 750 characters) are required. The company is optional. The language must
    /// already exist. Only the owner can do it.
    /// </remarks>
    /// <param name="request">The owner's data and the language in which the job role and the presentation are written.</param>
    /// <response code="201">Profile created.</response>
    /// <response code="400">A required field is empty, a text is too long or the language code is not valid.</response>
    /// <response code="401">The request has no valid access token. Sign in again or refresh the token.</response>
    /// <response code="403">The signed-in user is not the owner of the portfolio, so they cannot make changes.</response>
    /// <response code="404">The language does not exist. Create it first.</response>
    /// <response code="503">The profile cannot be saved right now.</response>
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult> Create(CreateProfileRequest request)
    {
        return await (
            from _ in creator.Execute(
                new ProfileCreatorInfo(
                    request.Name,
                    request.JobRole,
                    request.Presentation,
                    request.EnterpriseName,
                    request.LanguageCode
                )
            ).ToAsync()
            select Unit.Default
        ).Match(
            _ => CreatedAtAction(nameof(Create), request),
            error => errorResponseMapper.ToActionResult(this, error)
        );
    }
}