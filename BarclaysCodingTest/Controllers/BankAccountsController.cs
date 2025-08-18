using BarclaysCodingTest.Dtos;
using BarclaysCodingTest.Services;
using BarclaysCodingTest.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarclaysCodingTest.Controllers;

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

    [HttpGet]
    public IActionResult GetAll()
    {
        var result = bankAccountService.GetAll();

        if (result.Error is Error error)
        {
            return FromError(error);
        }

        return Ok(result.Value);        
    }

    [HttpGet("{id:guid}")]
    public IActionResult Get(Guid id)
    {
        var result = bankAccountService.Get(id);

        if (result.Error is Error error)
        {
            return FromError(error);
        }

        return Ok(result.Value);        
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateBankAccountRequest request)
    {
        var result = await bankAccountService.Update(id, request);

        if (result.Error is Error error)
        {
            return FromError(error);
        }

        return Ok(result.Value);        
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await bankAccountService.Delete(id);

        if (result.Error is Error error)
        {
            return FromError(error);
        }
	
        return NoContent();
    }

    [HttpPost("{accountId:guid}/transactions")]
    public async Task<IActionResult> CreateTransaction(Guid accountId, CreateTransactionRequest request)
    {
        var result = await bankAccountService.CreateTransaction(accountId, request);

        if (result.Error is Error error)
        {
            return FromError(error);
        }
	
        return NoContent();
    }

    [HttpGet("{accountId:guid}/transactions")]
    public IActionResult GetAll(Guid accountId)
    {
        var result = bankAccountService.GetAllTransactions(accountId);

        if (result.Error is Error error)
        {
            return FromError(error);
        }

        return Ok(result.Value);        
    }

    [HttpGet("{accountId:guid}/transactions/{transactionId:guid}")]
    public IActionResult Get(Guid accountId, Guid transactionId)
    {
        var result = bankAccountService.GetTransaction(accountId, transactionId);

        if (result.Error is Error error)
        {
            return FromError(error);
        }

        return Ok(result.Value);        
    }
}
