using BarclaysCodingTest.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarclaysCodingTest.Database.Entities;

public class BankAccountEntityTypeConfiguration
{
    public void Configure(EntityTypeBuilder<BankAccountEntity> builder)
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
            .Property(b => b.UserId)
            .HasColumnName("user_id")
            .IsRequired();
    }
}

