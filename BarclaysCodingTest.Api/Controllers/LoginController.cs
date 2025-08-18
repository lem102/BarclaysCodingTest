using BarclaysCodingTest.Api.Dtos;
using BarclaysCodingTest.Api.Services;
using BarclaysCodingTest.Api.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace BarclaysCodingTest.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class LoginController(ILoginService loginService) : BaseController
{
    [HttpPost]
    public IActionResult Login(LoginUserRequest request)
    {
        var result = loginService.Login(request);

        if (result.Error is Error error)
        {
            return FromError(error);
        }

        return Ok(result.Value);
    }

}




