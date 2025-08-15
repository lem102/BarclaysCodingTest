using BarclaysCodingTest.Dtos;
using BarclaysCodingTest.Entities;
using BarclaysCodingTest.Repository;
using BarclaysCodingTest.Utilities;
using Microsoft.AspNetCore.Identity;

namespace BarclaysCodingTest.Services;

public class UserService(
    IPasswordHasher<UserEntity> passwordHasher,
    IRepository<UserEntity> repository
) : IUserService
{
    public async Task<Result<CreateUserResponse>> Create(CreateUserRequest request)
    {
        var user = new UserEntity
        {
            Name = request.name,
        };

        var hashedPassword = passwordHasher.HashPassword(user, request.password);

        user.Password = hashedPassword;

        var createdUser = await repository.AddAsync(user);
        await repository.SaveChangesAsync();

        return Map(createdUser);
    }

    public Result<CreateUserResponse> Get(Guid id)
    {
        var nullableUser = repository.GetAll().FirstOrDefault(u => u.Id == id);

        if (nullableUser is not UserEntity user)
        {
            return Errors.UserNotFound(id);
        }

        return Map(user);
    }

    public Task<Result<LoginUserResponse>> Login(LoginUserRequest request)
    {
        throw new NotImplementedException();
    }

    private CreateUserResponse Map(UserEntity user)
    {
        return new CreateUserResponse(user.Id, user.Name, user.Password);
    }
}
