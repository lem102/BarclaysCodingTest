using BarclaysCodingTest.Dtos;
using BarclaysCodingTest.Utilities;

namespace BarclaysCodingTest.Entities;

public class UserEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
}
