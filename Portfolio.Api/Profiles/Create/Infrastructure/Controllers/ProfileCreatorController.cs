using LanguageExt;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Profiles.Create.Application.Dtos;
using Portfolio.Api.Profiles.Create.Application.UseCases;
using Portfolio.Api.Profiles.Create.Infrastructure.Dtos;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Profiles.Create.Infrastructure.Controllers;

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
                new ProfileCreatorInfo(
                    request.Name,
                    request.JobRole,
                    request.Presentation,
                    request.EnterpriseName,
                    request.LanguageCode
                )
            ).ToAsync()
            select Unit.Default
        ).Match<ActionResult>(
            _ => CreatedAtAction(nameof(Create), request),
            error => error.Exception.Case switch
            {
                NotFoundException => NotFound(new ProblemDetails
                {
                    Title = "Not Found",
                    Detail = error.Message,
                    Status = StatusCodes.Status404NotFound
                }),
                ValidationException => BadRequest(new ProblemDetails
                {
                    Title = "Bad Request",
                    Detail = error.Message,
                    Status = StatusCodes.Status400BadRequest
                }),
                _ => Problem()
            }
        );
    }
}