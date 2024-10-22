using BasedGram.Services.AuthService;
using BasedGram.Services.AuthService.Exceptions;
using BasedGram.Tests.Common.DataBuilders.Core;

namespace BasedGram.Tests.Unit.Services;

public class TestAuthService : TestServiceBase
{
    private readonly AuthService m_authService;

    public TestAuthService()
    {
        m_authService = new(m_mockUserRepo, null);
    }

    [Fact]
    public async Task LoginUser_ShouldReturnUser_WhenCredentialsAreCorrect()
    {
        // Arrange
        var user = new UserBuilder()
            .WithLogin("TestLogin")
            .WithPasswordHash(BCrypt.Net.BCrypt.HashPassword("TestHash"))
            .Build();
        m_users = [user];

        // Act
        var result = await m_authService.LoginUser(user.Login, "TestHash");

        // Assert
        Assert.Equal(user, result);
    }

    [Fact]
    public async Task LoginUser_ShouldThrowIncorrectPasswordException_WhenPasswordIsIncorrect()
    {
        // Arrange
        var user = new UserBuilder()
            .WithLogin("TestLogin")
            .WithPasswordHash(BCrypt.Net.BCrypt.HashPassword("TestHash"))
            .Build();
        m_users = [user];

        // Act
        var action = async () => await m_authService.LoginUser(user.Login, "WrongHash");

        // Assert
        await Assert.ThrowsAsync<IncorrectPasswordException>(action);
    }

    [Fact]
    public async Task LoginUser_ShouldThrowUserLoginNotFoundException_WhenUserNotFound()
    {
        // Arrange
        var user = new UserBuilder()
            .WithLogin("TestLogin")
            .WithPasswordHash(BCrypt.Net.BCrypt.HashPassword("TestHash"))
            .Build();

        // Act
        var action = async () => await m_authService.LoginUser(user.Login, "WrongHash");

        // Assert
        await Assert.ThrowsAsync<UserLoginNotFoundException>(action);
    }

    [Fact]
    public async Task RegisterUser_ShouldCreateUser_WhenUserDoesNotExist()
    {
        // Arrange

        // Act
        await m_authService.RegisterUser("test", "test");

        // Assert
        Assert.Single(m_users);
    }

    [Fact]
    public async Task RegisterUser_ShouldThrowUserRegisterAlreadyExistsException_WhenUserAlreadyExists()
    {
        // Arrange
        var user = new UserBuilder()
            .WithLogin("TestLogin")
            .WithPasswordHash(BCrypt.Net.BCrypt.HashPassword("TestHash"))
            .Build();

        m_users = [user];

        // Act
        var action = async () => await m_authService.RegisterUser("TestLogin", "TestHash");

        // Assert
        await Assert.ThrowsAsync<UserRegisterAlreadyExistsException>(action);
    }
}