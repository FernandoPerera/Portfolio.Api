using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Shared.Infrastructure.Ports;

public interface IErrorResponseMapper
{
    ActionResult ToActionResult(ControllerBase controller, DomainError error);
}
