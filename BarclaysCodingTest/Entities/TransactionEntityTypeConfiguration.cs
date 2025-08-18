using BarclaysCodingTest.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarclaysCodingTest.Entities;

public class TransactionEntityTypeConfiguration
{
    public void Configure(EntityTypeBuilder<TransactionEntity> builder)
    {
        builder.ToTable("transactions");

        builder
            .HasKey(t => t.Id);

        builder
            .Property(t => t.Id)
            .HasColumnName("id")
            .IsRequired();

        builder
            .Property(t => t.Amount)
            .HasColumnName("amount")
            .IsRequired();

        builder
            .Property(b => b.TransactionType)
            .HasColumnName("transaction_type")
            .HasConversion<string>()
            .IsRequired();

        builder
            .Property(b => b.BankAccountId)
            .HasColumnName("bank_account_id")
            .IsRequired();


        builder
            .HasOne<BankAccountEntity>(t => t.BankAccount)
            .WithMany(b => b.Transactions)
            .HasForeignKey(t => t.BankAccountId)
            .IsRequired();
    }
}
