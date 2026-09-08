using LanguageExt;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Profile.Create.Application.Dtos;
using Portfolio.Api.Profile.Create.Application.UseCases;
using Portfolio.Api.Profile.Create.Infrastructure.Dtos;

namespace Portfolio.Api.Profile.Create.Infrastructure.Controllers;

[ApiController]
[Route("v1/profile")]
public class ProfileCreatorController(ProfileCreator creator) : ControllerBase
{
    [HttpPost("create")]
    [Produces("application/json")]
    public async Task<ActionResult> Create(CreateProfileRequest request)
    {
        return await (
            from _ in creator.Execute(
                new ProfileCreatorInfo(request.Name, request.Role, request.Presentation, request.Enterprise)
            ).ToAsync()
            select Unit.Default
        ).Match<ActionResult>(
            _ => CreatedAtAction(nameof(Create), request),
            error => Problem()
        );
    }
}