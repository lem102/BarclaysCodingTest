namespace BarclaysCodingTest.Api.Entities;

public class UserEntity : BaseEntity
{
    public string Name { get; set; } = null!;

    public string Password { get; set; } = null!;
}
