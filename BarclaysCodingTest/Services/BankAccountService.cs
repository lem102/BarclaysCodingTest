using BarclaysCodingTest.Dtos;
using BarclaysCodingTest.Entities;
using BarclaysCodingTest.Database.Repository;
using BarclaysCodingTest.Utilities;
using BarclaysCodingTest.Enums;
using Microsoft.EntityFrameworkCore;

namespace BarclaysCodingTest.Services;

public class BankAccountService(
    IRepository<BankAccountEntity> Repository,
    IUserService UserService,
    IUserProvider UserProvider
) : IBankAccountService
{
    public async Task<Result<BankAccountResponse>> Create(CreateBankAccountRequest request)
    {
        var userResult = UserService.Get(UserProvider.GetCurrentUserId());

        if (userResult.Error is Error error)
        {
            return error;
        }

        var user = userResult.Value!;

        var bankAccountEntity = new BankAccountEntity
        {
            Name = request.Name,
            UserId = user.Id
        };

        var addedEntity = Repository.Add(bankAccountEntity);
        await Repository.SaveChangesAsync();

        return Map(addedEntity);
    }

    public Result<IEnumerable<BankAccountResponse>> GetAll()
    {
        var userId = UserProvider.GetCurrentUserId();

        var bankAccounts = Repository.GetAll().Where(b => b.UserId == userId);

        return bankAccounts.Select(Map).ToList();
    }

    public Result<BankAccountResponse> Get(Guid id)
    {
        var nullableBankAccount = Repository.GetAll().SingleOrDefault(b => b.Id == id);

        if (nullableBankAccount is not BankAccountEntity bankAccount)
        {
            return Errors.BankAccountNotFound(id);
        }

        var userId = UserProvider.GetCurrentUserId();

        if (bankAccount.UserId != userId)
        {
            return Errors.UserUnauthorized(userId);
        }

        return Map(bankAccount);
    }

    public async Task<Result<BankAccountResponse>> Update(Guid id, UpdateBankAccountRequest request)
    {
        var nullableBankAccount = Repository.GetAll().SingleOrDefault(b => b.Id == id);

        if (nullableBankAccount is not BankAccountEntity bankAccount)
        {
            return Errors.BankAccountNotFound(id);
        }

        var userId = UserProvider.GetCurrentUserId();

        if (bankAccount.UserId != userId)
        {
            return Errors.UserUnauthorized(userId);
        }

        if (request.Name is string newName)
        {
            bankAccount.Name = newName;
        }

        var updatedBankAccount = Repository.Update(bankAccount);
        await Repository.SaveChangesAsync();

        return Map(updatedBankAccount);
    }

    public async Task<Result> Delete(Guid id)
    {
        var nullableBankAccount = Repository.GetAll().SingleOrDefault(b => b.Id == id);

        if (nullableBankAccount is not BankAccountEntity bankAccount)
        {
            return Errors.BankAccountNotFound(id);
        }

        var userId = UserProvider.GetCurrentUserId();

        if (bankAccount.UserId != userId)
        {
            return Errors.UserUnauthorized(userId);
        }

        Repository.Delete(bankAccount);
        await Repository.SaveChangesAsync();

        return Result.Success();
    }

    private BankAccountResponse Map(BankAccountEntity entity)
    {
        return new BankAccountResponse(entity.Id, entity.UserId, entity.Name);
    }

    public async Task<Result> CreateTransaction(Guid accountId, CreateTransactionRequest request)
    {
        var nullableBankAccount = Repository
            .GetAll()
            .Include(b => b.Transactions)
            .SingleOrDefault(b => b.Id == accountId);

        if (nullableBankAccount is not BankAccountEntity bankAccount)
        {
            return Errors.BankAccountNotFound(accountId);
        }

        var userId = UserProvider.GetCurrentUserId();

        if (bankAccount.UserId != userId)
        {
            return Errors.UserUnauthorized(userId);
        }

        var transactionEntity = new TransactionEntity
        {
            Amount = request.Amount,
            TransactionType = request.TransactionType
        };

        bankAccount.Transactions.Add(transactionEntity);

        if (!Enum.IsDefined(request.TransactionType))
        {
            return Errors.InvalidTransactionType(request.TransactionType);
        }

        bankAccount.Balance = request switch
        {
            { TransactionType: TransactionType.Deposit } => bankAccount.Balance + request.Amount,
            _ => bankAccount.Balance - request.Amount,
        };

        if (bankAccount.Balance < 0)
        {
            return Errors.InsufficientFunds(bankAccount.Id);
        }

        Repository.Update(bankAccount);
        await Repository.SaveChangesAsync();
        
        return Result.Success();
    }

    public Result<IEnumerable<TransactionResponse>> GetAllTransactions(Guid accountId)
    {
        throw new NotImplementedException();
    }

    public Result<TransactionResponse> GetTransaction(Guid accountId, Guid transactionId)
    {
        throw new NotImplementedException();
    }
}
