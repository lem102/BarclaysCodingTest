using BarclaysCodingTest.Dtos;
using BarclaysCodingTest.Services;
using BarclaysCodingTest.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarclaysCodingTest.Dtos;

public record BankAccountResponse(Guid Id, Guid UserId, string Name);
