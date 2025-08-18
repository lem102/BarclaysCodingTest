using BarclaysCodingTest.Enums;

namespace BarclaysCodingTest.Dtos;

public record TransactionResponse(Guid Id, Guid BankAccountId, TransactionType TransactionType, int Amount);

