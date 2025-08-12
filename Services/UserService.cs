using BarclaysCodingTest.Dtos;
using BarclaysCodingTest.Entities;
using BarclaysCodingTest.Repository;
using BarclaysCodingTest.Utilities;

namespace BarclaysCodingTest.Services;

public class UserService(IRepository<UserEntity> repository) : IUserService
{
    public Result<CreateUserResponse> Create(CreateUserRequest request)
    {
        var createdUser = repository.Add(MapRequestToEntity(request));

        return MapEntityToResponse(createdUser);
    }

    private UserEntity MapRequestToEntity(CreateUserRequest request)
    {
        return new UserEntity
        {
            Name = request.name,
            Password = request.password
        };
    }

    private CreateUserResponse MapEntityToResponse(UserEntity entity)
    {
        return new CreateUserResponse(entity.Id, entity.Name, entity.Password);
    }
}
