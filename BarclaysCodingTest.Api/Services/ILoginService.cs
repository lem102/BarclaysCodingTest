using BarclaysCodingTest.Api.Dtos;
using BarclaysCodingTest.Api.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace BarclaysCodingTest.Api.Services;

public interface ILoginService
{
    Result<string> Login(LoginUserRequest request);
}
