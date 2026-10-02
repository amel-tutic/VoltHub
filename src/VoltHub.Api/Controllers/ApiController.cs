using Microsoft.AspNetCore.Mvc;
using VoltHub.Api.Contracts;
using VoltHub.Application.Common.Results;

namespace VoltHub.Api.Controllers;

[ApiController]
public abstract class ApiController : ControllerBase
{
    // 200 with the value, or an RFC 7807 problem.
    protected IActionResult FromResult<T>(Result<T> result) =>
        result.IsSuccess ? Ok(result.Value) : Problem(result.Error!);

    // 204 No Content, or an RFC 7807 problem.
    protected IActionResult FromResult(Result result) =>
        result.IsSuccess ? NoContent() : Problem(result.Error!);

    // 201 Created with the new id, or an RFC 7807 problem.
    protected IActionResult FromCreated(Result<Guid> result) =>
        result.IsSuccess ? StatusCode(StatusCodes.Status201Created, new CreatedResponse(result.Value)) : Problem(result.Error!);

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
