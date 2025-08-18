using Moq;
using BarclaysCodingTest.Dtos;
using BarclaysCodingTest.Entities;
using BarclaysCodingTest.Database.Repository;
using BarclaysCodingTest.Utilities;
using Microsoft.AspNetCore.Identity;
using BarclaysCodingTest.Services;

namespace BarclaysCodingTest.Test.Services;

[TestClass]
public class UserServiceTests
{
    private Mock<IPasswordHasher<UserEntity>> _mockPasswordHasher = new();
    private Mock<IRepository<UserEntity>> _mockRepository = new();
    private Mock<IUserProvider> _mockUserProvider = new();
    private UserService _sut;

    private List<UserEntity> _users = new List<UserEntity>
    {
        new UserEntity {
            Id = Guid.NewGuid(),
            Name = "testuser1",
            Password = "hashedpassword1",
            BankAccounts = new List<BankAccountEntity>() {}
        },
        new UserEntity {
            Id = Guid.NewGuid(),
            Name = "testuser2",
            Password = "hashedpassword2",
            BankAccounts = new List<BankAccountEntity>() {new BankAccountEntity{}}

        }
    };

    [TestInitialize]
    public void Setup()
    {
        _sut = new UserService(
            _mockPasswordHasher.Object,
            _mockRepository.Object,
            _mockUserProvider.Object
        );
    }

    [TestMethod]
    public async Task Create_ValidRequest_ReturnsSuccessAndAddsUser()
    {
        // Arrange
        var request = new CreateUserRequest("newuser", "securepassword");
        var newUser = new UserEntity { Id = Guid.NewGuid(), Name = "newuser" };
        var hashedPassword = "hashednewpassword";

        _mockRepository.Setup(r => r.GetAll()).Returns(_users.AsQueryable());

        _mockPasswordHasher.Setup(p => p.HashPassword(It.IsAny<UserEntity>(), It.IsAny<string>()))
                           .Returns(hashedPassword);

        _mockRepository.Setup(r => r.Add(It.IsAny<UserEntity>()))
                       .Returns(newUser);

        // Act
        var result = await _sut.Create(request);

        // Assert
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(newUser.Id, result.Value.Id);
        Assert.AreEqual(newUser.Name, result.Value.Name);

        _mockRepository.Verify(
            r => r.Add(
                It.Is<UserEntity>(u => u.Name == "newuser" && u.Password == hashedPassword)
            ),
            Times.Once
        );
     
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [TestMethod]
    public async Task Create_UsernameExists_ReturnsUsernameUnavailableError()
    {
        // Arrange
        var existingUser = _users.First();
        var request = new CreateUserRequest(existingUser.Name, "securepassword");

        _mockRepository.Setup(r => r.GetAll()).Returns(_users.AsQueryable());

        // Act
        var result = await _sut.Create(request);

        // Assert
        Assert.AreEqual(Errors.UsernameUnavailable, result);

        _mockRepository.Verify(r => r.Add(It.IsAny<UserEntity>()), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [TestMethod]
    public void Get_UserExists_ReturnsSuccessWithUser()
    {
        // Arrange
        var existingUser = _users.First();

        _mockUserProvider.Setup(up => up.GetCurrentUserId()).Returns(existingUser.Id);

        _mockRepository.Setup(r => r.GetAll()).Returns(_users.AsQueryable());

        // Act
        var result = _sut.Get(existingUser.Id);

        // Assert
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(existingUser.Id, result.Value.Id);
        Assert.AreEqual(existingUser.Name, result.Value.Name);
    }

    [TestMethod]
    public void Get_UserDoesNotExist_ReturnsUserNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        _mockUserProvider.Setup(up => up.GetCurrentUserId()).Returns(nonExistentId);

        _mockRepository.Setup(r => r.GetAll()).Returns(_users.AsQueryable());

        // Act
        var result = _sut.Get(nonExistentId);

        // Assert
        Assert.AreEqual(Errors.UserNotFound(nonExistentId), result.Error);
    }

    [TestMethod]
    public void Get_UserRequestingDifferentUser_ReturnsNotAuthorized()
    {
        // Arrange
        var loggedInUser = _users.First();
        var userIdInRequest = _users.Last();

        _mockUserProvider.Setup(up => up.GetCurrentUserId()).Returns(loggedInUser.Id);
        _mockRepository.Setup(r => r.GetAll()).Returns(_users.AsQueryable());

        // Act
        var result = _sut.Get(userIdInRequest.Id);

        // Assert
        Assert.AreEqual(Errors.UserUnauthorized(loggedInUser.Id), result.Error);
    }

    [TestMethod]
    public async Task Update_ValidRequest_UpdatesUserSuccessfully()
    {
        // Arrange
        var userToUpdate = _users.First();
        var newName = "updateduser1";
        var newPassword = "newsecurepassword";
        var hashedPassword = "newhashedpassword";

        var request = new UpdateUserRequest(newName, newPassword);

        _mockUserProvider.Setup(up => up.GetCurrentUserId()).Returns(userToUpdate.Id);
        _mockRepository.Setup(r => r.GetAll()).Returns(_users.AsQueryable());

        _mockPasswordHasher.Setup(p => p.HashPassword(It.IsAny<UserEntity>(), newPassword))
                           .Returns(hashedPassword);

        _mockRepository.Setup(r => r.Update(It.IsAny<UserEntity>()))
                       .Returns<UserEntity>(u =>
                       {
                           u.Name = newName;
                           u.Password = hashedPassword;
                           return u;
                       });

        // Act
        var result = await _sut.Update(userToUpdate.Id, request);

        // Assert
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(userToUpdate.Id, result.Value.Id);
        Assert.AreEqual(newName, result.Value.Name);

        _mockRepository.Verify(
            r => r.Update(
                It.Is<UserEntity>(u => u.Id == userToUpdate.Id && u.Name == newName && u.Password == hashedPassword)
            ),
            Times.Once
        );

        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [TestMethod]
    public async Task Update_UserDoesNotExist_ReturnsUserNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var request = new UpdateUserRequest("nonexistentuser", "newpassword");

        _mockUserProvider.Setup(up => up.GetCurrentUserId()).Returns(nonExistentId);
        _mockRepository.Setup(r => r.GetAll()).Returns(_users.AsQueryable());

        // Act
        var result = await _sut.Update(nonExistentId, request);

        // Assert
        Assert.AreEqual(Errors.UserNotFound(nonExistentId), result.Error);

        _mockRepository.Verify(r => r.Update(It.IsAny<UserEntity>()), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [TestMethod]
    public async Task Update_UserIsUnauthorized_ReturnsNotAuthorized()
    {
        // Arrange
        var loggedInUser = _users.First();
        var userToUpdate = _users.Last();
        var request = new UpdateUserRequest("unauthorizeduser", "newpassword");

        _mockUserProvider.Setup(up => up.GetCurrentUserId()).Returns(loggedInUser.Id);
        _mockRepository.Setup(r => r.GetAll()).Returns(_users.AsQueryable());

        // Act
        var result = await _sut.Update(userToUpdate.Id, request);

        // Assert
        Assert.AreEqual(Errors.UserUnauthorized(loggedInUser.Id), result.Error);

        _mockRepository.Verify(r => r.Update(It.IsAny<UserEntity>()), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [TestMethod]
    public async Task Delete_ValidRequest_DeletesUserSuccessfully()
    {
        // Arrange
        var userToDelete = _users.First();

        _mockUserProvider.Setup(up => up.GetCurrentUserId()).Returns(userToDelete.Id);
        _mockRepository.Setup(r => r.GetAll()).Returns(_users.AsQueryable());

        // Act
        var result = await _sut.Delete(userToDelete.Id);

        // Assert
        Assert.IsNull(result.Error);

        _mockRepository.Verify(r => r.Delete(It.Is<UserEntity>(u => u.Id == userToDelete.Id)), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [TestMethod]
    public async Task Delete_UserDoesNotExist_ReturnsUserNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        _mockUserProvider.Setup(up => up.GetCurrentUserId()).Returns(nonExistentId);
        _mockRepository.Setup(r => r.GetAll()).Returns(_users.AsQueryable());

        // Act
        var result = await _sut.Delete(nonExistentId);

        // Assert
        Assert.AreEqual(Errors.UserNotFound(nonExistentId), result.Error);

        _mockRepository.Verify(r => r.Delete(It.IsAny<UserEntity>()), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [TestMethod]
    public async Task Delete_UserIsUnauthorized_ReturnsNotAuthorized()
    {
        // Arrange
        var loggedInUser = _users.First();
        var userToDelete = _users.Last();

        _mockUserProvider.Setup(up => up.GetCurrentUserId()).Returns(loggedInUser.Id);
        _mockRepository.Setup(r => r.GetAll()).Returns(_users.AsQueryable());

        // Act
        var result = await _sut.Delete(userToDelete.Id);

        // Assert
        Assert.AreEqual(Errors.UserUnauthorized(loggedInUser.Id), result.Error);

        _mockRepository.Verify(r => r.Delete(It.IsAny<UserEntity>()), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [TestMethod]
    public async Task Delete_UserHasBankAccount_ReturnsError()
    {
        // Arrange
        var userToDelete = _users[1];

        _mockUserProvider.Setup(up => up.GetCurrentUserId()).Returns(userToDelete.Id);
        _mockRepository.Setup(r => r.GetAll()).Returns(_users.AsQueryable());

        // Act
        var result = await _sut.Delete(userToDelete.Id);

        // Assert
        Assert.AreEqual(Errors.UserHasBankAccountPreventingDeletion(userToDelete.Id), result.Error);

        _mockRepository.Verify(r => r.Delete(It.Is<UserEntity>(u => u.Id == userToDelete.Id)), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
}
