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
public class ProfileFinderController(
    ProfileFinder finder,
    IErrorResponseMapper errorResponseMapper
) : ControllerBase
{
    [HttpGet]
    [Produces("application/json")]
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
