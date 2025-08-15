using Microsoft.AspNetCore.Mvc;

namespace BarclaysCodingTest.Dtos;

public record CreateUserRequest(string Name, string password);
