using BarclaysCodingTest.Entities;
using Microsoft.EntityFrameworkCore;

namespace BarclaysCodingTest.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserEntity> users { get; set; }
}
