using System.Security.Claims;
using BarclaysCodingTest.Dtos;
using BarclaysCodingTest.Entities;
using BarclaysCodingTest.Repository;
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
        var existingUserWithName = repository.GetAll().FirstOrDefault(u => u.Name == request.Name);

        if (existingUserWithName is not null)
        {
            return Errors.UsernameUnavailable;
        }

        var user = new UserEntity
        {
            Name = request.Name,
        };

        var hashedPassword = passwordHasher.HashPassword(user, request.password);

        user.Password = hashedPassword;

        var createdUser = await repository.AddAsync(user);

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
        
        var nullableUser = repository.GetAll().FirstOrDefault(u => u.Id == id);

        if (nullableUser is not UserEntity user)
        {
            return Errors.UserNotFound(id);
        }

        return Map(user);
    }

    private UserResponse Map(UserEntity user)
    {
        return new UserResponse(user.Id, user.Name, user.Password);
    }
}

public interface IUserProvider
{
    Guid GetCurrentUserId();
}

public class UserProvider(IHttpContextAccessor httpContextAccessor) : IUserProvider
{
    public Guid GetCurrentUserId()
    {
        var httpContext = httpContextAccessor.HttpContext;

        var userIdClaim = httpContext?.User
            .FindFirst(ClaimTypes.NameIdentifier);

        var nullableUserId = userIdClaim?.Value;

        return nullableUserId switch {
            string userId => Guid.Parse(userId),
            _ => Guid.Empty,
        };
    }
}
