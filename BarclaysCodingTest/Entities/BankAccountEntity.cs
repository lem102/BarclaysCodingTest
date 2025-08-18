namespace BarclaysCodingTest.Entities;

public class BankAccountEntity : BaseEntity
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = null!;
}
