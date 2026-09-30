using LanguageExt;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Profiles.Create.Application.Dtos;
using Portfolio.Api.Profiles.Create.Application.UseCases;
using Portfolio.Api.Profiles.Create.Infrastructure.Dtos;
using Portfolio.Api.Shared.Infrastructure.Ports;

namespace Portfolio.Api.Profiles.Create.Infrastructure.Controllers;

[ApiController]
[Route("v1/profile")]
public class ProfileCreatorController(
    ProfileCreator creator,
    IErrorResponseMapper errorResponseMapper
) : ControllerBase
{
    [HttpPost("create")]
    [Produces("application/json")]
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