using BarclaysCodingTest.Entities;
using BarclaysCodingTest.Database.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace BarclaysCodingTest.Database;

public class ApplicationDbContext(IConfiguration configuration) : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserEntity>().ToTable("users");
        modelBuilder.Entity<UserEntity>().HasIndex(u => u.Name).IsUnique();
        new UserEntityTypeConfiguration().Configure(modelBuilder.Entity<UserEntity>());

        modelBuilder.Entity<BankAccountEntity>().ToTable("bank_accounts");
        new BankAccountEntityTypeConfiguration().Configure(modelBuilder.Entity<BankAccountEntity>());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
    }

    public DbSet<UserEntity> Users { get; set; } = null!;
}
