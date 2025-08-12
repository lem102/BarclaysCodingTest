using BarclaysCodingTest.Dtos;
using BarclaysCodingTest.Services;
using Microsoft.AspNetCore.Mvc;

namespace BarclaysCodingTest.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService) : BaseController
{
    public IActionResult Create(CreateUserRequest request)
    {
        var result = userService.Create(request);
        return FromResult(result);
    }
}
