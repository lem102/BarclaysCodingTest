using BarclaysCodingTest.Dtos;
using BarclaysCodingTest.Entities;
using BarclaysCodingTest.Database.Repository;
using BarclaysCodingTest.Utilities;

namespace BarclaysCodingTest.Services;

public class BankAccountService(
    IRepository<BankAccountEntity> repository,
    IUserService userService,
    IUserProvider userProvider
) : IBankAccountService
{
    public async Task<Result<BankAccountResponse>> Create(CreateBankAccountRequest request)
    {
        var userResult = userService.Get(userProvider.GetCurrentUserId());

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

        var addedEntity = repository.Add(bankAccountEntity);
        await repository.SaveChangesAsync();

        return Map(addedEntity);
    }

    private BankAccountResponse Map(BankAccountEntity entity)
    {
        return new BankAccountResponse(entity.Id, entity.UserId, entity.Name);
    }
}
