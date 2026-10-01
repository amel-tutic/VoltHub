using Microsoft.AspNetCore.Mvc;
using VoltHub.Application.Common.Results;

namespace VoltHub.Api.Controllers;

[ApiController]
public abstract class ApiController : ControllerBase
{
    // Turns an expected business failure into an RFC 7807 problem response.
    protected ObjectResult Problem(Error error) => Problem(
        detail: error.Message,
        statusCode: error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        },
        title: error.Code);
}
