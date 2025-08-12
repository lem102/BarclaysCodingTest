namespace BarclaysCodingTest.Entities;

public class UserEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Password { get; set; } = null!;
}
