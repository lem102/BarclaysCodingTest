using BarclaysCodingTest.Dtos;
using BarclaysCodingTest.Services;
using BarclaysCodingTest.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarclaysCodingTest.Services;

public interface IBankAccountService
{
    Task<Result<BankAccountResponse>> Create(CreateBankAccountRequest request);
}
