using BarclaysCodingTest.Dtos;
using BarclaysCodingTest.Entities;
using BarclaysCodingTest.Database.Repository;
using BarclaysCodingTest.Utilities;
using Microsoft.AspNetCore.Identity;

namespace BarclaysCodingTest.Services;

public class UserService(
    IPasswordHasher<UserEntity> PasswordHasher,
    IRepository<UserEntity> Repository,
    IUserProvider UserProvider
) : IUserService
{
    public async Task<Result<UserResponse>> Create(CreateUserRequest request)
    {
        var existingUserWithName = Repository.GetAll().SingleOrDefault(u => u.Name == request.Name);

        if (existingUserWithName is not null)
        {
            return Errors.UsernameUnavailable;
        }

        var user = new UserEntity
        {
            Name = request.Name,
        };

        var hashedPassword = PasswordHasher.HashPassword(user, request.Password);

        user.Password = hashedPassword;

        var createdUser = Repository.Add(user);

        await Repository.SaveChangesAsync();

        return Map(createdUser);
    }

    public Result<UserResponse> Get(Guid id)
    {
        var currentUserId = UserProvider.GetCurrentUserId();
        
        if (!currentUserId.Equals(id))
        {
            return Errors.UserUnauthorized(currentUserId);
        }
        
        var nullableUser = Repository.GetAll().SingleOrDefault(u => u.Id == id);

        if (nullableUser is not UserEntity user)
        {
            return Errors.UserNotFound(id);
        }

        return Map(user);
    }

    public async Task<Result<UserResponse>> Update(Guid id, UpdateUserRequest request)
    {
        var currentUserId = UserProvider.GetCurrentUserId();
        
        if (!currentUserId.Equals(id))
        {
            return Errors.UserUnauthorized(currentUserId);
        }

        var nullableUser = Repository.GetAll().SingleOrDefault(u => u.Id == id);

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
            var hashedPassword = PasswordHasher.HashPassword(user, request.Password);
            user.Password = hashedPassword;
        }

        var updatedUser = Repository.Update(user);
        await Repository.SaveChangesAsync();

        return Map(updatedUser);
    }

    public async Task<Result> Delete(Guid id)
    {
        var currentUserId = UserProvider.GetCurrentUserId();

        if (!currentUserId.Equals(id))
        {
            return Errors.UserUnauthorized(currentUserId);
        }

        var nullableUser = Repository.GetAll().SingleOrDefault(u => u.Id == id);

        if (nullableUser is not UserEntity user)
        {
            return Errors.UserNotFound(id);
        }

        Repository.Delete(user);
        await Repository.SaveChangesAsync();

        return Result.Success();
    }

    private UserResponse Map(UserEntity user)
    {
        return new UserResponse(user.Id, user.Name);
    }
}

