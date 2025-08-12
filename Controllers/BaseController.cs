using BarclaysCodingTest.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace BarclaysCodingTest.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BaseController : ControllerBase
{
    public IActionResult FromResult<T>(Result<T> result)
    {
        return result.Error?.Type switch
        {
            null => Ok(result.Value),
            _ => FromResult(result.Error)
        };
    }

    public IActionResult FromResult(Result result)
    {
        return result.Error?.Type switch
        {
            null => Ok(),
            ErrorType.NotFound => NotFound(result.Error.Description),
            ErrorType.Validation => BadRequest(result.Error.Description),
            _ => StatusCode(500, result.Error?.Description)
        };
    }
}
