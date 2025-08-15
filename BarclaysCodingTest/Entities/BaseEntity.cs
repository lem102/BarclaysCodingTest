using BarclaysCodingTest.Database;
using Microsoft.EntityFrameworkCore;

namespace BarclaysCodingTest.Entities;

public class BaseEntity
{
    public Guid Id { get; set; }
}
