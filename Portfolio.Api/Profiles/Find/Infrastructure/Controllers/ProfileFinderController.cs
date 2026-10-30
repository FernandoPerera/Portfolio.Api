using LanguageExt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Portfolio.Api.Profiles.Find.Application.UseCases;
using Portfolio.Api.Profiles.Find.Infrastructure.Dtos;
using Portfolio.Api.Shared.Infrastructure.Extensions;
using Portfolio.Api.Shared.Infrastructure.Ports;

namespace Portfolio.Api.Profiles.Find.Infrastructure.Controllers;

[ApiController]
[AllowAnonymous]
[EnableRateLimiting(RateLimitingExtensions.PublicPolicy)]
[Route("v1/profile")]
[Tags("Profile")]
public class ProfileFinderController(
    ProfileFinder finder,
    IErrorResponseMapper errorResponseMapper
) : ControllerBase
{
    /// <summary>
    /// Shows the profile of the portfolio owner.
    /// </summary>
    /// <remarks>
    /// Returns the owner's name and company, plus the presentation and the job role in every available
    /// language.
    /// </remarks>
    /// <response code="200">The profile with all its translations.</response>
    /// <response code="404">The profile has not been created yet.</response>
    /// <response code="429">Too many requests from the same address. Wait for the number of seconds in the `Retry-After` header and try again.</response>
    /// <response code="503">The profile cannot be read right now.</response>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType<ProfileResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<ProfileResponse>> Find()
    {
        return await (
            from result in finder.Execute().ToAsync()
            select new ProfileResponse(
                result.GetName(),
                result.GetEnterprise(),
                result.GetTranslations()
                    .Select(translation => new ProfileTranslationResponse(
                        translation.GetLanguageCode(),
                        translation.GetPresentation(),
                        translation.GetRole()
                    ))
                    .ToList()
            )
        ).Match<ActionResult<ProfileResponse>>(
            profile => Ok(profile),
            error => errorResponseMapper.ToActionResult(this, error)
        );
    }
}
