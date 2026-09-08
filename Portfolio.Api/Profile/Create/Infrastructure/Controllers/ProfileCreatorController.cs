using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Profile.Create.Application.UseCases;
using Portfolio.Api.Profile.Create.Infrastructure.Dtos;

namespace Portfolio.Api.Profile.Create.Infrastructure.Controllers;

[ApiController]
[Route("v1/profile")]
public class ProfileCreatorController(ProfileCreator creator)
{
    [HttpPost("create")]
    [Produces("application/json")]
    public async Task<ActionResult> Create(CreateProfileRequest request)
    {
        throw new NotImplementedException();
    }
}