using BarclaysCodingTest.Api.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace BarclaysCodingTest.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BaseController : ControllerBase
{
    public IActionResult FromError(Error error)
    {
        return error.Type switch
        {
            ErrorType.NotFound => NotFound(error.Description),
            ErrorType.Validation => BadRequest(error.Description),
            ErrorType.Unauthorized => Unauthorized(error.Description),
            _ => StatusCode(500, error.Description)
        };
    }
}
