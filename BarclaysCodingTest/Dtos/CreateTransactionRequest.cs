using System.Text.Json.Serialization;
using BarclaysCodingTest.Enums;

namespace BarclaysCodingTest.Dtos;

public record CreateTransactionRequest
{
    [JsonRequired]
    public required TransactionType TransactionType { get; init; }

    [JsonRequired]
    public required int Amount { get; init; }
}
