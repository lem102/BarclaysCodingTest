using BarclaysCodingTest.Api.Dtos;
using BarclaysCodingTest.Api.Services;
using BarclaysCodingTest.Api.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarclaysCodingTest.Api.Services;

public interface IBankAccountService
{
    Task<Result<BankAccountResponse>> Create(CreateBankAccountRequest request);
}
