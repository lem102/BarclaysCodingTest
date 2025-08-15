using BarclaysCodingTest.Dtos;
using BarclaysCodingTest.Utilities;

namespace BarclaysCodingTest.Services;

public interface IUserService
{
    Task<Result<CreateUserResponse>> Create(CreateUserRequest request);
    Result<CreateUserResponse> Get(Guid id);
    Task<Result<LoginUserResponse>> Login(LoginUserRequest request);
}
