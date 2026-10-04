using LanguageExt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Languages.Create.Application.Dtos;
using Portfolio.Api.Languages.Create.Application.UseCases;
using Portfolio.Api.Languages.Create.Infrastructure.Dtos;
using Portfolio.Api.Shared.Infrastructure.Ports;

namespace Portfolio.Api.Languages.Create.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("v1/language")]
public class LanguageCreatorController(
    LanguageCreator creator,
    IErrorResponseMapper errorResponseMapper
) : ControllerBase
{
    [HttpPost]
    [Produces("application/json")]
    public async Task<ActionResult> Create(CreateLanguageRequest request)
    {
        return await (
            from _ in creator.Execute(
                new LanguageCreatorInfo(request.LanguageCode, request.Name)
            ).ToAsync()
            select Unit.Default
        ).Match(
            _ => CreatedAtAction(nameof(Create), request),
            error => errorResponseMapper.ToActionResult(this, error)
        );
    }
}
