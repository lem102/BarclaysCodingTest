using BarclaysCodingTest.Enums;

namespace BarclaysCodingTest.Dtos;

public record CreateTransactionRequest(TransactionType TransactionType, int Amount);
