using BarclaysCodingTest.Api.Dtos;
using BarclaysCodingTest.Api.Services;
using BarclaysCodingTest.Api.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarclaysCodingTest.Api.Dtos;

public record CreateBankAccountRequest(string Name);
