using LanguageExt;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Profiles.Find.Application.Dtos;
using Portfolio.Api.Profiles.Find.Application.UseCases;
using Portfolio.Api.Profiles.Find.Infrastructure.Dtos;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Profiles.Find.Infrastructure.Controllers;

[ApiController]
[Route("v1/profile")]
public class ProfileFinderController(ProfileFinder finder) : ControllerBase
{
    [HttpGet]
    [Produces("application/json")]
    public async Task<ActionResult<ProfileResponse>> Find(
        [FromHeader(Name = "Accept-Language")] string acceptLanguage = "es"
    )
    {
        return await (
            from result in finder.Execute(new ProfileFinderInfo(acceptLanguage)).ToAsync()
            select new ProfileResponse(
                result.GetName(),
                result.GetPresentation(),
                result.GetRole(),
                result.GetEnterprise()
            )
        ).Match<ActionResult<ProfileResponse>>(
            profile => Ok(profile),
            error => error.Exception.Case switch
            {
                NotFoundException => NotFound(new ProblemDetails
                {
                    Title = "Not Found",
                    Detail = error.Message,
                    Status = StatusCodes.Status404NotFound
                }),
                _ => Problem()
            }
        );
    }
}
