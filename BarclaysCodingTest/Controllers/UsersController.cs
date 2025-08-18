using BarclaysCodingTest.Dtos;
using BarclaysCodingTest.Services;
using BarclaysCodingTest.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarclaysCodingTest.Controllers;

[ApiController]
[Route("/v1/[controller]")]
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
	
        return StatusCode(201, result.Value);
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

        return Ok(result.Value);
    }

    [Authorize]
    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateUserRequest request)
    {
        var result = await userService.Update(id, request);

        if (result.Error is Error error)
        {
            return FromError(error);
        }

        return Ok(result.Value);
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await userService.Delete(id);

        if (result.Error is Error error)
        {
            return FromError(error);
        }

        return NoContent();
    }
}
