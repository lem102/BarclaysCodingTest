using BarclaysCodingTest.Dtos;
using BarclaysCodingTest.Utilities;

namespace BarclaysCodingTest.Dtos;

public record CreateUserResponse(Guid id, string name, string password);
