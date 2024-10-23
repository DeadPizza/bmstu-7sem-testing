using Abp.Collections.Extensions;
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

namespace BasedGram.Tests.Integration.Repositories;

[Collection("Test Database")]
public class TestUserRepository : TestRepositoryBase
{
    protected readonly UserRepository m_userRepository;

    public TestUserRepository(DatabaseFixture fixture) : base(fixture)
    {
        m_userRepository = new UserRepository(m_dbFixture.CreateContext());
    }

    [Fact]
    public async Task CreateUser_Ok()
    {
        // Arrange
        var context = m_dbFixture.CreateContext();
        var user = UserDbModelFactory.Create(Guid.NewGuid(), "TestUser", "TestHash", 0, false, false);

        // Act
        await m_userRepository.CreateUser(UserConverter.DbToCoreModel(user));

        // Assert
        var models = (from a in context.Users select a).ToList();
        Assert.Single(models);
        Assert.Equivalent(user, models[0]);
    }

    [Fact]
    public async Task CreateUser_NonUniqueException()
    {
        // Arrange
        var user = (await CreateDefaultUsers())[0];

        // Act
        async Task action() => await m_userRepository.CreateUser(UserConverter.DbToCoreModel(user));

        // Assert
        await Assert.ThrowsAsync<DbUpdateException>(action);
    }

    [Fact]
    public async Task DeleteUser_Ok()
    {
        // Arrange
        var user = await CreateSingleUser();

        // Act
        await m_userRepository.DeleteUser(UserConverter.DbToCoreModel(user));

        // Assert
        Assert.Empty(m_dbFixture.CreateContext().Users);
    }

    [Fact]
    public async Task DeleteUser_NotFound()
    {
        // Arrange
        var user = UserDbModelFactory.Create(Guid.NewGuid(), "TestUser", "TestHash", 0, false, false);

        // Act
        await m_userRepository.DeleteUser(UserConverter.DbToCoreModel(user));

        // Assert
        Assert.Empty(m_dbFixture.CreateContext().Users);
    }

    [Fact]
    public async Task GetUserByID_Ok()
    {
        // Arrange
        var user = await CreateSingleUser();

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

        // Act
        var result = await m_userRepository.GetUserByID(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ListAllAdmins_Ok()
    {
        // Arrange
        var admin1 = await CreateSingleUserFrom(UserDbModelFactory.Create(Guid.NewGuid(), "Admin1", "TestHash1", 1, false, false));
        var admin2 = await CreateSingleUserFrom(UserDbModelFactory.Create(Guid.NewGuid(), "Admin2", "TestHash2", 1, false, false));

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

        // Act
        var admins = await m_userRepository.ListAllAdmins();

        // Assert
        Assert.Empty(admins);
    }

    [Fact]
    public async Task ListAllUsers_Ok()
    {
        // Arrange
        List<UserDbModel> models = await CreateDefaultUsers();
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

        // Act
        var users = await m_userRepository.ListAllUsers();

        // Assert
        Assert.Empty(users);
    }

    [Fact]
    public async Task UpdateUser_Ok()
    {
        // Arrange
        var user = await CreateSingleUser();

        // Act
        user.Login = "UpdatedUser";
        await m_userRepository.UpdateUser(UserConverter.DbToCoreModel(user));

        // Assert
        Assert.Equal("UpdatedUser", (from a in m_dbFixture.CreateContext().Users select a).ToList()[0].Login);
    }

    [Fact]
    public async Task UpdateUser_AddNew()
    {
        // Arrange
        var user = UserDbModelFactory.Create(Guid.NewGuid(), "User1", "TestHash", 0, false, false);

        // Act
        await m_userRepository.UpdateUser(UserConverter.DbToCoreModel(user));

        // Assert
        var models = (from a in m_dbFixture.CreateContext().Users select a).ToList();
        Assert.Single(models);
        Assert.Equal(user.Login, models[0].Login);
    }
}