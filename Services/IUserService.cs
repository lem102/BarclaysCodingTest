using BarclaysCodingTest.Dtos;
using BarclaysCodingTest.Utilities;

namespace BarclaysCodingTest.Services;

public interface IUserService
{
    public Result<CreateUserResponse> Create(CreateUserRequest request);
}
