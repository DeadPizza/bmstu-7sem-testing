using BasedGram.Common.Enums;
using BasedGram.Database.Npgsql.Models;
using BasedGram.Database.Npgsql.Models.Converters;
using BasedGram.Database.NpgsqlRepositories;
using BasedGram.Tests.Common.Factories.Db;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using Serilog;
using Xunit;

namespace BasedGram.Tests.Unit.Repositories;
public class TestUserRepository : TestRepositoryBase
{
    protected readonly UserRepository m_userRepository;

    public TestUserRepository() : base()
    {
        m_userRepository = new UserRepository(m_mockDbContextFactory.MockContext.Object);
    }

    [Fact]
    public async Task CreateUser_Ok()
    {
        // Arrange
        List<UserDbModel> models = new();
        var user = UserDbModelFactory.Create(Guid.NewGuid(), "TestUser", "TestHash", 0, false, false);

        m_mockDbContextFactory
            .MockUsers.Setup(s => s.AddAsync(It.IsAny<UserDbModel>(), default))
            .Callback<UserDbModel, CancellationToken>((a, token) => models.Add(a));

        // Act
        await m_userRepository.CreateUser(UserConverter.DbToCoreModel(user));

        // Assert
        Assert.Single(models);
        Assert.Equivalent(user, models[0]);
    }

    [Fact]
    public async Task CreateUser_NonUniqueException()
    {
        // Arrange
        var user = UserDbModelFactory.Create(Guid.NewGuid(), "TestUser", "TestHash", 0, false, false);

        m_mockDbContextFactory
            .MockUsers.Setup(s => s.AddAsync(It.IsAny<UserDbModel>(), default))
            .Callback<UserDbModel, CancellationToken>((a, token) => throw new Exception());

        // Act
        async Task action() => await m_userRepository.CreateUser(UserConverter.DbToCoreModel(user));

        // Assert
        await Assert.ThrowsAsync<Exception>(action);
    }

    [Fact]
    public async Task DeleteUser_Ok()
    {
        // Arrange
        var user = UserDbModelFactory.Create(Guid.NewGuid(), "TestUser", "TestHash", 0, false, false);
        List<UserDbModel> models = new() { user };

        m_mockDbContextFactory
            .MockUsers.Setup(s => s.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync(user);

        m_mockDbContextFactory
            .MockUsers.Setup(s => s.Remove(It.IsAny<UserDbModel>()))
            .Callback<UserDbModel>(a => models.Remove(a));

        // Act
        await m_userRepository.DeleteUser(UserConverter.DbToCoreModel(user));

        // Assert
        Assert.Empty(models);
    }

    [Fact]
    public async Task DeleteUser_NotFound()
    {
        // Arrange
        var user = UserDbModelFactory.Create(Guid.NewGuid(), "TestUser", "TestHash", 0, false, false);

        m_mockDbContextFactory
            .MockUsers.Setup(s => s.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync((UserDbModel)null);

        // Act
        await m_userRepository.DeleteUser(UserConverter.DbToCoreModel(user));

        // Assert
        m_mockDbContextFactory.MockUsers.Verify(s => s.Remove(It.IsAny<UserDbModel>()), Times.Never);
    }

    [Fact]
    public async Task GetUserByID_Ok()
    {
        // Arrange
        var user = UserDbModelFactory.Create(Guid.NewGuid(), "TestUser", "TestHash", 0, false, false);

        m_mockDbContextFactory
            .MockUsers.Setup(s => s.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync(user);

        // Act
        var result = await m_userRepository.GetUserByID(user.ID);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Login, result.Login);
    }

    [Fact]
    public async Task GetUserByID_NotFound()
    {
        // Arrange
        m_mockDbContextFactory
            .MockUsers.Setup(s => s.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync((UserDbModel)null);

        // Act
        var result = await m_userRepository.GetUserByID(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ListAllAdmins_Ok()
    {
        // Arrange
        var admin1 = UserDbModelFactory.Create(Guid.NewGuid(), "Admin1", "TestHash", 1, false, false);
        var admin2 = UserDbModelFactory.Create(Guid.NewGuid(), "Admin2", "TestHash", 1, false, false);
        List<UserDbModel> models = new() { admin1, admin2 };

        m_mockDbContextFactory
            .MockContext
                .Setup(x => x.Users)
                .ReturnsDbSet(models);

        // Act
        var admins = await m_userRepository.ListAllAdmins();

        // Assert
        Assert.Equal(2, admins.Count);
        Assert.True(admins.All(a => a.Role == Role.Admin));
    }

    [Fact]
    public async Task ListAllAdmins_Empty()
    {
        // Arrange
        List<UserDbModel> models = new();

        m_mockDbContextFactory
            .MockContext
                .Setup(x => x.Users)
                .ReturnsDbSet(models);

        // Act
        var admins = await m_userRepository.ListAllAdmins();

        // Assert
        Assert.Empty(admins);
    }

    [Fact]
    public async Task ListAllUsers_Ok()
    {
        // Arrange
        var user1 = UserDbModelFactory.Create(Guid.NewGuid(), "User1", "TestHash", 0, false, false);
        var user2 = UserDbModelFactory.Create(Guid.NewGuid(), "User1", "TestHash", 0, false, false);
        List<UserDbModel> models = new() { user1, user2 };

        m_mockDbContextFactory
            .MockContext
                .Setup(x => x.Users)
                .ReturnsDbSet(models);

        // Act
        var users = await m_userRepository.ListAllUsers();

        // Assert
        Assert.Equal(2, users.Count);
        Assert.True(users.All(u => u.Role == 0));
    }

    [Fact]
    public async Task ListAllUsers_Empty()
    {
        // Arrange
        List<UserDbModel> models = new();

        m_mockDbContextFactory
            .MockContext
                .Setup(x => x.Users)
                .ReturnsDbSet(models);

        // Act
        var users = await m_userRepository.ListAllUsers();

        // Assert
        Assert.Empty(users);
    }

    [Fact]
    public async Task UpdateUser_Ok()
    {
        // Arrange
        var user = UserDbModelFactory.Create(Guid.NewGuid(), "User1", "TestHash", 0, false, false);
        List<UserDbModel> models = new() { user };

        m_mockDbContextFactory
            .MockUsers.Setup(s => s.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync(user);

        // Act
        user.Login = "UpdatedUser";
        await m_userRepository.UpdateUser(UserConverter.DbToCoreModel(user));

        // Assert
        Assert.Equal("UpdatedUser", models[0].Login);
    }

    [Fact]
    public async Task UpdateUser_AddNew()
    {
        // Arrange
        var user = UserDbModelFactory.Create(Guid.NewGuid(), "User1", "TestHash", 0, false, false);
        List<UserDbModel> models = new();

        m_mockDbContextFactory
            .MockUsers.Setup(s => s.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync((UserDbModel)null);

        m_mockDbContextFactory
            .MockUsers.Setup(s => s.AddAsync(It.IsAny<UserDbModel>(), default))
            .Callback<UserDbModel, CancellationToken>((a, token) => models.Add(a));

        // Act
        await m_userRepository.UpdateUser(UserConverter.DbToCoreModel(user));

        // Assert
        Assert.Single(models);
        Assert.Equal(user.Login, models[0].Login);
    }
}