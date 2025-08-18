using BarclaysCodingTest.Enums;

namespace BarclaysCodingTest.Dtos;

public record TransactionResponse(Guid BankAccountId, TransactionType TransactionType, int Amount);

