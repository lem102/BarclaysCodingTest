using BarclaysCodingTest.Api.Dtos;
using BarclaysCodingTest.Api.Services;
using BarclaysCodingTest.Api.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarclaysCodingTest.Api.Controllers;

[ApiController]
[Authorize]
[Route("/v1/[controller]")]
public class BankAccountsController(IBankAccountService bankAccountService) : BaseController
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateBankAccountRequest request)
    {
        var result = await bankAccountService.Create(request);

        if (result.Error is Error error)
        {
            return FromError(error);
        }
	
        return StatusCode(201, result.Value);
    }
}
