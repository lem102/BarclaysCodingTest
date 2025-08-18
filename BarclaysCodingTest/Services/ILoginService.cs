using BarclaysCodingTest.Dtos;
using BarclaysCodingTest.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace BarclaysCodingTest.Services;

public interface ILoginService
{
    Result<string> Login(LoginUserRequest request);
}
