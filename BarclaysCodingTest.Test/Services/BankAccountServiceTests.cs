using Moq;
using BarclaysCodingTest.Dtos;
using BarclaysCodingTest.Entities;
using BarclaysCodingTest.Database.Repository;
using BarclaysCodingTest.Utilities;
using BarclaysCodingTest.Enums;
using BarclaysCodingTest.Services;

namespace BarclaysCodingTest.Test.Services;

[TestClass]
public class BankAccountServiceTests
{
    private Mock<IRepository<BankAccountEntity>> _mockRepository = new();
    private Mock<IUserService> _mockUserService = new();
    private Mock<IUserProvider> _mockUserProvider = new();
    private BankAccountService _sut;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _otherUserId = Guid.NewGuid();

    private List<BankAccountEntity> _bankAccounts = new();

    [TestInitialize]
    public void Setup()
    {
        _bankAccounts = new List<BankAccountEntity>
        {
            new BankAccountEntity { Id = Guid.NewGuid(), Name = "testAccount1", UserId = _userId, Balance = 100 },
            new BankAccountEntity { Id = Guid.NewGuid(), Name = "testAccount2", UserId = _userId, Balance = 250 },
            new BankAccountEntity { Id = Guid.NewGuid(), Name = "otherUserAccount", UserId = _otherUserId, Balance = 50 }
        };

        _sut = new BankAccountService(
            _mockRepository.Object,
            _mockUserService.Object,
            _mockUserProvider.Object
        );

        _mockUserProvider.Setup(up => up.GetCurrentUserId()).Returns(_userId);
        _mockRepository.Setup(r => r.GetAll()).Returns(_bankAccounts.AsQueryable());
    }

    [TestMethod]
    public async Task Create_ValidRequest_ReturnsSuccessAndAddsBankAccount()
    {
        // Arrange
        var request = new CreateBankAccountRequest("newAccount");
        var newUser = new UserResponse(Id: _userId, Name: "testuser");
        var newBankAccountEntity = new BankAccountEntity { Id = Guid.NewGuid(), Name = "newAccount", UserId = _userId };

        _mockUserService.Setup(us => us.Get(It.IsAny<Guid>())).Returns(newUser);
        _mockRepository.Setup(r => r.Add(It.IsAny<BankAccountEntity>())).Returns(newBankAccountEntity);

        // Act
        var result = await _sut.Create(request);

        // Assert
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(newBankAccountEntity.Id, result.Value.Id);
        Assert.AreEqual(newBankAccountEntity.Balance, 0);
        Assert.AreEqual("newAccount", result.Value.Name);
        Assert.AreEqual(_userId, result.Value.UserId);

        _mockRepository.Verify(r => r.Add(It.Is<BankAccountEntity>(b => b.Name == "newAccount" && b.UserId == _userId)), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [TestMethod]
    public async Task Create_UserDoesNotExist_ReturnsError()
    {
        // Arrange
        var request = new CreateBankAccountRequest("newAccount");
        var error = Errors.UserNotFound(_userId);
        _mockUserService.Setup(us => us.Get(It.IsAny<Guid>())).Returns(error);

        // Act
        var result = await _sut.Create(request);

        // Assert
        Assert.AreEqual(error, result.Error);
        _mockRepository.Verify(r => r.Add(It.IsAny<BankAccountEntity>()), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [TestMethod]
    public void GetAll_ReturnsAllBankAccountsForCurrentUser()
    {
        // Act
        var result = _sut.GetAll();

        // Assert
        Assert.IsNotNull(result.Value);
        var bankAccounts = result.Value.ToList();
        Assert.AreEqual(2, bankAccounts.Count);
        Assert.IsTrue(bankAccounts.All(b => b.UserId == _userId));
    }

    [TestMethod]
    public void Get_BankAccountExistsForCurrentUser_ReturnsSuccessWithBankAccount()
    {
        // Arrange
        var existingAccount = _bankAccounts.First(b => b.UserId == _userId);

        // Act
        var result = _sut.Get(existingAccount.Id);

        // Assert
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(existingAccount.Id, result.Value.Id);
        Assert.AreEqual(existingAccount.Name, result.Value.Name);
    }

    [TestMethod]
    public void Get_BankAccountDoesNotExist_ReturnsBankAccountNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = _sut.Get(nonExistentId);

        // Assert
        Assert.AreEqual(Errors.BankAccountNotFound(nonExistentId), result.Error);
    }

    [TestMethod]
    public void Get_BankAccountBelongsToAnotherUser_ReturnsUnauthorized()
    {
        // Arrange
        var otherUserAccount = _bankAccounts.First(b => b.UserId == _otherUserId);

        // Act
        var result = _sut.Get(otherUserAccount.Id);

        // Assert
        Assert.AreEqual(Errors.UserUnauthorized(_userId), result.Error);
    }

    [TestMethod]
    public async Task Update_ValidRequest_UpdatesBankAccountSuccessfully()
    {
        // Arrange
        var accountToUpdate = _bankAccounts.First(b => b.UserId == _userId);
        var newName = "updatedAccountName";
        var request = new UpdateBankAccountRequest(newName);

        _mockRepository.Setup(r => r.Update(It.IsAny<BankAccountEntity>())).Returns<BankAccountEntity>(b => b);

        // Act
        var result = await _sut.Update(accountToUpdate.Id, request);

        // Assert
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(newName, result.Value.Name);

        _mockRepository.Verify(
            r => r.Update(It.Is<BankAccountEntity>(b => b.Id == accountToUpdate.Id && b.Name == newName)),
            Times.Once
        );
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [TestMethod]
    public async Task Update_BankAccountDoesNotExist_ReturnsBankAccountNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var request = new UpdateBankAccountRequest("nonExistentAccount");

        // Act
        var result = await _sut.Update(nonExistentId, request);

        // Assert
        Assert.AreEqual(Errors.BankAccountNotFound(nonExistentId), result.Error);
        _mockRepository.Verify(r => r.Update(It.IsAny<BankAccountEntity>()), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [TestMethod]
    public async Task Update_BankAccountBelongsToAnotherUser_ReturnsUnauthorized()
    {
        // Arrange
        var otherUserAccount = _bankAccounts.First(b => b.UserId == _otherUserId);
        var request = new UpdateBankAccountRequest("unauthorizedUpdate");

        // Act
        var result = await _sut.Update(otherUserAccount.Id, request);

        // Assert
        Assert.AreEqual(Errors.UserUnauthorized(_userId), result.Error);
        _mockRepository.Verify(r => r.Update(It.IsAny<BankAccountEntity>()), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [TestMethod]
    public async Task Delete_ValidRequest_DeletesBankAccountSuccessfully()
    {
        // Arrange
        var accountToDelete = _bankAccounts.First(b => b.UserId == _userId);
        _mockRepository.Setup(r => r.Delete(It.IsAny<BankAccountEntity>()));

        // Act
        var result = await _sut.Delete(accountToDelete.Id);

        // Assert
        Assert.IsNull(result.Error);
        _mockRepository.Verify(r => r.Delete(It.Is<BankAccountEntity>(b => b.Id == accountToDelete.Id)), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [TestMethod]
    public async Task Delete_BankAccountDoesNotExist_ReturnsBankAccountNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _sut.Delete(nonExistentId);

        // Assert
        Assert.AreEqual(Errors.BankAccountNotFound(nonExistentId), result.Error);
        _mockRepository.Verify(r => r.Delete(It.IsAny<BankAccountEntity>()), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [TestMethod]
    public async Task Delete_BankAccountBelongsToAnotherUser_ReturnsUnauthorized()
    {
        // Arrange
        var otherUserAccount = _bankAccounts.First(b => b.UserId == _otherUserId);

        // Act
        var result = await _sut.Delete(otherUserAccount.Id);

        // Assert
        Assert.AreEqual(Errors.UserUnauthorized(_userId), result.Error);
        _mockRepository.Verify(r => r.Delete(It.IsAny<BankAccountEntity>()), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [TestMethod]
    public async Task CreateTransaction_Deposit_IncreasesBalanceAndAddsTransaction()
    {
        // Arrange
        var account = _bankAccounts.First(b => b.UserId == _userId);
        account.Transactions = new List<TransactionEntity>();

        var request = new CreateTransactionRequest
        {
            Amount = 100,
            TransactionType = TransactionType.Deposit
        };
    
        _mockRepository.Setup(r => r.GetAll()).Returns(_bankAccounts.AsQueryable());
        _mockRepository.Setup(r => r.Update(It.IsAny<BankAccountEntity>())).Returns(account);
        var initialBalance = account.Balance;

        // Act
        var result = await _sut.CreateTransaction(account.Id, request);

        // Assert
        Assert.IsNull(result.Error);
        Assert.AreEqual(initialBalance + request.Amount, account.Balance);
        Assert.AreEqual(1, account.Transactions.Count);
        Assert.AreEqual(request.Amount, account.Transactions.First().Amount);
        Assert.AreEqual(request.TransactionType, account.Transactions.First().TransactionType);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [TestMethod]
    public async Task CreateTransaction_Withdrawal_DecreasesBalanceAndAddsTransaction()
    {
        // Arrange
        var account = _bankAccounts.First(b => b.UserId == _userId);
        account.Transactions = new List<TransactionEntity>();

        var request = new CreateTransactionRequest
        {
            Amount = 50,
            TransactionType = TransactionType.Withdrawal
        };
        
        _mockRepository.Setup(r => r.GetAll()).Returns(_bankAccounts.AsQueryable());
        _mockRepository.Setup(r => r.Update(It.IsAny<BankAccountEntity>())).Returns(account);
        var initialBalance = account.Balance;

        // Act
        var result = await _sut.CreateTransaction(account.Id, request);

        // Assert
        Assert.IsNull(result.Error);
        Assert.AreEqual(initialBalance - request.Amount, account.Balance);
        Assert.AreEqual(1, account.Transactions.Count);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [TestMethod]
    public async Task CreateTransaction_InsufficientFunds_ReturnsError()
    {
        // Arrange
        var account = _bankAccounts.First(b => b.UserId == _userId);
        account.Transactions = new List<TransactionEntity>();

        var request = new CreateTransactionRequest
        {
            Amount = 200,
            TransactionType = TransactionType.Withdrawal
        };
        
        _mockRepository.Setup(r => r.GetAll()).Returns(_bankAccounts.AsQueryable());

        // Act
        var result = await _sut.CreateTransaction(account.Id, request);

        // Assert
        Assert.AreEqual(Errors.InsufficientFunds(account.Id), result.Error);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [TestMethod]
    public async Task CreateTransaction_BankAccountDoesNotExist_ReturnsBankAccountNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        var request = new CreateTransactionRequest{
            Amount = 10,
            TransactionType = TransactionType.Deposit
        };
        
        _mockRepository.Setup(r => r.GetAll()).Returns(_bankAccounts.AsQueryable());

        // Act
        var result = await _sut.CreateTransaction(nonExistentId, request);

        // Assert
        Assert.AreEqual(Errors.BankAccountNotFound(nonExistentId), result.Error);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [TestMethod]
    public async Task CreateTransaction_BankAccountBelongsToAnotherUser_ReturnsUnauthorized()
    {
        // Arrange
        var otherUserAccount = _bankAccounts.First(b => b.UserId == _otherUserId);

        var request = new CreateTransactionRequest{
            Amount = 10,
            TransactionType = TransactionType.Deposit
        };
        
        _mockRepository.Setup(r => r.GetAll()).Returns(_bankAccounts.AsQueryable());

        // Act
        var result = await _sut.CreateTransaction(otherUserAccount.Id, request);

        // Assert
        Assert.AreEqual(Errors.UserUnauthorized(_userId), result.Error);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [TestMethod]
    public async Task CreateTransaction_InvalidTransactionType_ReturnsError()
    {
        // Arrange
        var account = _bankAccounts.First(b => b.UserId == _userId);
        account.Transactions = new List<TransactionEntity>();

        var request = new CreateTransactionRequest{
            Amount = 10,
            TransactionType = (TransactionType)999
        };
        
        _mockRepository.Setup(r => r.GetAll()).Returns(_bankAccounts.AsQueryable());

        // Act
        var result = await _sut.CreateTransaction(account.Id, request);

        // Assert
        Assert.AreEqual(Errors.InvalidTransactionType(request.TransactionType), result.Error);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [TestMethod]
    public void GetAllTransactions_BankAccountExistsForCurrentUser_ReturnsAllTransactions()
    {
        // Arrange
        var account = _bankAccounts.First(b => b.UserId == _userId);
        account.Transactions = new List<TransactionEntity>
        {
            new TransactionEntity { Id = Guid.NewGuid(), Amount = 10, TransactionType = TransactionType.Deposit },
            new TransactionEntity { Id = Guid.NewGuid(), Amount = 5, TransactionType = TransactionType.Withdrawal }
        };
        _mockRepository.Setup(r => r.GetAll()).Returns(_bankAccounts.AsQueryable());

        // Act
        var result = _sut.GetAllTransactions(account.Id);

        // Assert
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(2, result.Value.Count());
    }

    [TestMethod]
    public void GetAllTransactions_BankAccountDoesNotExist_ReturnsBankAccountNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = _sut.GetAllTransactions(nonExistentId);

        // Assert
        Assert.AreEqual(Errors.BankAccountNotFound(nonExistentId), result.Error);
    }

    [TestMethod]
    public void GetAllTransactions_BankAccountBelongsToAnotherUser_ReturnsUnauthorized()
    {
        // Arrange
        var otherUserAccount = _bankAccounts.First(b => b.UserId == _otherUserId);

        // Act
        var result = _sut.GetAllTransactions(otherUserAccount.Id);

        // Assert
        Assert.AreEqual(Errors.UserUnauthorized(_userId), result.Error);
    }

    [TestMethod]
    public void GetTransaction_ValidRequest_ReturnsSuccessWithTransaction()
    {
        // Arrange
        var account = _bankAccounts.First(b => b.UserId == _userId);
        var transaction = new TransactionEntity { Id = Guid.NewGuid(), Amount = 10, TransactionType = TransactionType.Deposit };
        account.Transactions = new List<TransactionEntity> { transaction };
        _mockRepository.Setup(r => r.GetAll()).Returns(_bankAccounts.AsQueryable());

        // Act
        var result = _sut.GetTransaction(account.Id, transaction.Id);

        // Assert
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(transaction.Id, result.Value.Id);
        Assert.AreEqual(transaction.Amount, result.Value.Amount);
    }

    [TestMethod]
    public void GetTransaction_BankAccountDoesNotExist_ReturnsBankAccountNotFound()
    {
        // Arrange
        var nonExistentBankAccountId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();
        _mockRepository.Setup(r => r.GetAll()).Returns(_bankAccounts.AsQueryable());

        // Act
        var result = _sut.GetTransaction(nonExistentBankAccountId, transactionId);

        // Assert
        Assert.AreEqual(Errors.BankAccountNotFound(nonExistentBankAccountId), result.Error);
    }

    [TestMethod]
    public void GetTransaction_TransactionDoesNotExist_ReturnsTransactionNotFound()
    {
        // Arrange
        var account = _bankAccounts.First(b => b.UserId == _userId);
        account.Transactions = new List<TransactionEntity>();
        var nonExistentTransactionId = Guid.NewGuid();
        _mockRepository.Setup(r => r.GetAll()).Returns(_bankAccounts.AsQueryable());

        // Act
        var result = _sut.GetTransaction(account.Id, nonExistentTransactionId);

        // Assert
        Assert.AreEqual(Errors.TransactionNotFound(nonExistentTransactionId), result.Error);
    }

    [TestMethod]
    public void GetTransaction_BankAccountBelongsToAnotherUser_ReturnsUnauthorized()
    {
        // Arrange
        var otherUserAccount = _bankAccounts.First(b => b.UserId == _otherUserId);
        var transactionId = Guid.NewGuid();
        _mockRepository.Setup(r => r.GetAll()).Returns(_bankAccounts.AsQueryable());

        // Act
        var result = _sut.GetTransaction(otherUserAccount.Id, transactionId);

        // Assert
        Assert.AreEqual(Errors.UserUnauthorized(_userId), result.Error);
    }
}
