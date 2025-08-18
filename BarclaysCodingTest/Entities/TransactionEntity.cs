using BarclaysCodingTest.Enums;

namespace BarclaysCodingTest.Entities;

public class TransactionEntity : BaseEntity
{
    public Guid BankAccountId { get; set; }
    public TransactionType TransactionType { get; set; }
    public int Amount { get; set; }

    public BankAccountEntity BankAccount { get; set; } = null!;
}
