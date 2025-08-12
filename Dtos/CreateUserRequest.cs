using Microsoft.AspNetCore.Mvc;

namespace BarclaysCodingTest.Dtos;

public record CreateUserRequest(string name, string password);
