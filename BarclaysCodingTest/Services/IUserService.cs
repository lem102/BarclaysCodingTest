using BarclaysCodingTest.Dtos;
using BarclaysCodingTest.Utilities;

namespace BarclaysCodingTest.Services;

public interface IUserService
{
    Task<Result<UserResponse>> Create(CreateUserRequest request);
    Result<UserResponse> Get(Guid id);
    Task<Result<UserResponse>> Update(Guid id, UpdateUserRequest request);
    Task<Result> Delete(Guid id);
}
