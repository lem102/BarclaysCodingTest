using System.Text.Json.Serialization;

namespace BarclaysCodingTest.Enums;

[JsonConverter(typeof(JsonStringEnumConverter<TransactionType>))]
public enum TransactionType
{
    Deposit,
    Withdrawal
}
