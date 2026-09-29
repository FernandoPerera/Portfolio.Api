using LanguageExt.Common;
using Microsoft.AspNetCore.Mvc;

namespace Portfolio.Api.Shared.Infrastructure.Ports;

public interface IErrorResponseMapper
{
    ActionResult ToActionResult(ControllerBase controller, Error error);
}
