using LanguageExt.Common;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Shared.Infrastructure.Ports;

public interface IErrorResponseMapper
{
    ActionResult ToActionResult(ControllerBase controller, Error error);

    ActionResult ToActionResult(ControllerBase controller, DomainError error);
}
