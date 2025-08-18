using BarclaysCodingTest.Entities;
using BarclaysCodingTest.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace BarclaysCodingTest.Database;

public class ApplicationDbContext(IConfiguration configuration) : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new UserEntityTypeConfiguration().Configure(modelBuilder.Entity<UserEntity>());
        new BankAccountEntityTypeConfiguration().Configure(modelBuilder.Entity<BankAccountEntity>());
        new TransactionEntityTypeConfiguration().Configure(modelBuilder.Entity<TransactionEntity>());}

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
    }

    public DbSet<UserEntity> Users { get; set; } = null!;
}
