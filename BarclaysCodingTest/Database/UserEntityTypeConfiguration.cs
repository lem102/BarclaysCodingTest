using BarclaysCodingTest.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarclaysCodingTest.Database;

public class UserEntityTypeConfiguration
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder
            .HasKey(b => b.Id);

        builder
            .Property(b => b.Id)
            .HasColumnName("id")
            .IsRequired();

        builder
            .Property(b => b.Name)
            .HasColumnName("name")
            .IsRequired();

        builder
            .Property(b => b.Password)
            .HasColumnName("password")
            .IsRequired();
    }
}

