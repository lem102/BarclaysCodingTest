namespace BarclaysCodingTest.Entities;

public class BankAccountEntity : BaseEntity
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = null!;
    public int Balance { get; set; }

    public UserEntity User { get; set; } = null!;
    public ICollection<TransactionEntity> Transactions { get; set; } = null!;
}
