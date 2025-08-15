using BarclaysCodingTest.Dtos;
using BarclaysCodingTest.Services;
using BarclaysCodingTest.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarclaysCodingTest.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController(IUserService userService) : BaseController
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateUserRequest request)
    {
        var result = await userService.Create(request);

        if (result.Error is Error error)
        {
            return FromError(error);
        }
	
        return StatusCode(201, result);
    }

    [Authorize]
    [HttpGet("{id:guid}")]
    public IActionResult Get(Guid id)
    {
        var result = userService.Get(id);

        if (result.Error is Error error)
        {
            return FromError(error);
        }

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> Login(LoginUserRequest request)
    {
	var result = await userService.Login(request);

	if (result.Error is Error error)
        {
            return FromError(error);
        }

        return Ok(result);
    }
}
