using BarclaysCodingTest.Dtos;
using BarclaysCodingTest.Entities;
using BarclaysCodingTest.Database.Repository;
using BarclaysCodingTest.Utilities;
using Microsoft.AspNetCore.Identity;

namespace BarclaysCodingTest.Services;

public class UserService(
    IPasswordHasher<UserEntity> passwordHasher,
    IRepository<UserEntity> repository,
    IUserProvider userProvider
) : IUserService
{
    public async Task<Result<UserResponse>> Create(CreateUserRequest request)
    {
        var existingUserWithName = repository.GetAll().SingleOrDefault(u => u.Name == request.Name);

        if (existingUserWithName is not null)
        {
            return Errors.UsernameUnavailable;
        }

        var user = new UserEntity
        {
            Name = request.Name,
        };

        var hashedPassword = passwordHasher.HashPassword(user, request.Password);

        user.Password = hashedPassword;

        var createdUser = repository.Add(user);

        await repository.SaveChangesAsync();

        return Map(createdUser);
    }

    public Result<UserResponse> Get(Guid id)
    {
        var currentUserId = userProvider.GetCurrentUserId();
        
        if (!currentUserId.Equals(id))
        {
            return Errors.UserUnauthorized(currentUserId);
        }
        
        var nullableUser = repository.GetAll().SingleOrDefault(u => u.Id == id);

        if (nullableUser is not UserEntity user)
        {
            return Errors.UserNotFound(id);
        }

        return Map(user);
    }

    public async Task<Result<UserResponse>> Update(Guid id, UpdateUserRequest request)
    {
        var currentUserId = userProvider.GetCurrentUserId();
        
        if (!currentUserId.Equals(id))
        {
            return Errors.UserUnauthorized(currentUserId);
        }

        var nullableUser = repository.GetAll().SingleOrDefault(u => u.Id == id);

        if (nullableUser is not UserEntity user)
        {
            return Errors.UserNotFound(id);
        }

        if (request.Name is string newName)
        {
            user.Name = newName;
        }

        if (request.Password is string newPassword)
        {
            var hashedPassword = passwordHasher.HashPassword(user, request.Password);
            user.Password = hashedPassword;
        }

        var updatedUser = repository.Update(user);
        await repository.SaveChangesAsync();

        return Map(updatedUser);
    }

    public async Task<Result> Delete(Guid id)
    {
        var currentUserId = userProvider.GetCurrentUserId();

        if (!currentUserId.Equals(id))
        {
            return Errors.UserUnauthorized(currentUserId);
        }

        var nullableUser = repository.GetAll().SingleOrDefault(u => u.Id == id);

        if (nullableUser is not UserEntity user)
        {
            return Errors.UserNotFound(id);
        }

        repository.Delete(user);
        await repository.SaveChangesAsync();

        return Result.Success();
    }

    private UserResponse Map(UserEntity user)
    {
        return new UserResponse(user.Id, user.Name);
    }
}

