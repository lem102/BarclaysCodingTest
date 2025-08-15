using Moq;
using BarclaysCodingTest.Dtos;
using BarclaysCodingTest.Entities;
using BarclaysCodingTest.Repository;
using BarclaysCodingTest.Utilities;
using Microsoft.AspNetCore.Identity;

namespace BarclaysCodingTest.Services.Tests;

[TestClass]
public class UserServiceTests
{
    private Mock<IPasswordHasher<UserEntity>> _mockPasswordHasher = new();
    private Mock<IRepository<UserEntity>> _mockRepository = new();
    private Mock<IUserProvider> _mockUserProvider = new();
    private UserService _sut;

    private List<UserEntity> _users = new List<UserEntity>
    {
        new UserEntity { Id = Guid.NewGuid(), Name = "testuser1", Password = "hashedpassword1" },
        new UserEntity { Id = Guid.NewGuid(), Name = "testuser2", Password = "hashedpassword2" }
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

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<UserEntity>()))
                       .ReturnsAsync(newUser);

        // Act
        var result = await _sut.Create(request);

        // Assert
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(newUser.Id, result.Value.Id);
        Assert.AreEqual(newUser.Name, result.Value.Name);
        Assert.AreEqual(newUser.Password, result.Value.Password);

        _mockRepository.Verify(
            r => r.AddAsync(
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

        _mockRepository.Verify(r => r.AddAsync(It.IsAny<UserEntity>()), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [TestMethod]
    public void Get_UserExists_ReturnsSuccessWithUser()
    {
        // Arrange
        var existingUser = _users.First();

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

        _mockRepository.Setup(r => r.GetAll()).Returns(_users.AsQueryable());

        // Act
        var result = _sut.Get(nonExistentId);

        // Assert
        Assert.AreEqual(Errors.UserNotFound(nonExistentId).Id, result.Error.Id);
    }
}
