using BarclaysCodingTest.Api.Dtos;
using BarclaysCodingTest.Api.Utilities;

namespace BarclaysCodingTest.Api.Services;

public interface IUserService
{
    Task<Result<UserResponse>> Create(CreateUserRequest request);
    Result<UserResponse> Get(Guid id);
    Task<Result<UserResponse>> Update(Guid id, UpdateUserRequest request);
    Task<Result> Delete(Guid id);
}
