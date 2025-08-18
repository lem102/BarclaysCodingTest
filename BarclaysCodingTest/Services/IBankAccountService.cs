using BarclaysCodingTest.Dtos;
using BarclaysCodingTest.Utilities;

namespace BarclaysCodingTest.Services;

public interface IBankAccountService
{
    Task<Result<BankAccountResponse>> Create(CreateBankAccountRequest request);
    Result<IEnumerable<BankAccountResponse>> GetAll();
    Result<BankAccountResponse> Get(Guid id);
    Task<Result<BankAccountResponse>> Update(Guid id, UpdateBankAccountRequest request);
    Task<Result> Delete(Guid id);
    
    Task<Result> CreateTransaction(Guid accountId, CreateTransactionRequest request);
    Result<IEnumerable<TransactionResponse>> GetAllTransactions(Guid accountId);
    Result<TransactionResponse> GetTransaction(Guid accountId, Guid transactionId);
}
