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
public class ProfileUpdaterController(
    ProfileUpdater updater,
    IErrorResponseMapper errorResponseMapper
) : ControllerBase
{
    [HttpPatch("{languageCode}")]
    [Produces("application/json")]
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
