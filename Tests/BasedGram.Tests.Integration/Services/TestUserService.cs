using BasedGram.Common.Core;
using BasedGram.Common.Enums;
using BasedGram.Services.UserService;
using BasedGram.Services.UserService.Exceptions;
using BasedGram.Tests.Common.DataBuilders.Core;
using BasedGram.Tests.Common.Factories.Core;

namespace BasedGram.Tests.Integration.Services;

[Collection("Test Database")]
public class TestUserService : TestServiceBase
{
    private readonly UserService m_userService;

    public TestUserService(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
        m_userService = new(m_userRepository, null);
    }

    [Fact]
    public async Task EraseUser_Ok()
    {
        // Arrange
        var user = new UserBuilder().WithID(Guid.NewGuid()).WithPasswordHash("A").WithLogin("A")
            .WithID(Guid.NewGuid())
            .Build();

        m_users = [user];

        // Act
        await m_userService.EraseUser(user);

        // Assert
        Assert.Empty(m_users);
    }

    [Fact]
    public async Task EraseUser_NotFound()
    {
        // Arrange
        var user = new UserBuilder().WithID(Guid.NewGuid()).WithPasswordHash("A").WithLogin("A")
            .WithID(Guid.NewGuid())
            .Build();

        // Act
        await m_userService.EraseUser(user);

        // Assert
        Assert.Empty(m_users);
    }

    [Fact]
    public async Task FreezeUser_Ok()
    {
        // Arrange
        var user = new UserBuilder().WithID(Guid.NewGuid()).WithPasswordHash("A").WithLogin("A")
            .WithIsFreezed(false)
            .Build();

        m_users = [user];

        // Act
        await m_userService.FreezeUser(UserFactory.Copy(user));

        // Assert
        Assert.False(user.IsFreezed);
    }

    [Fact]
    public async Task FindUser_Ok()
    {
        // Arrange
        var user1 = new UserBuilder().WithID(Guid.NewGuid()).WithPasswordHash("A").WithLogin("A")
            .WithLogin("a")
            .Build();

        var user2 = new UserBuilder().WithID(Guid.NewGuid()).WithPasswordHash("A").WithLogin("B")
             .WithLogin("b")
             .Build();

        var user3 = new UserBuilder().WithID(Guid.NewGuid()).WithPasswordHash("A").WithLogin("C")
             .WithLogin("ab")
             .Build();

        m_users = [user1, user2, user3];

        // Act
        var found = await m_userService.FindUser("a");

        // Assert
        Assert.Equivalent(found, new List<User>([user1, user3]));
    }

    [Fact]
    public async Task FindUser_Empty()
    {
        // Arrange
        var user1 = new UserBuilder().WithID(Guid.NewGuid()).WithPasswordHash("A").WithLogin("A")
            .WithLogin("a")
            .Build();

        var user2 = new UserBuilder().WithID(Guid.NewGuid()).WithPasswordHash("A").WithLogin("B")
             .WithLogin("b")
             .Build();

        var user3 = new UserBuilder().WithID(Guid.NewGuid()).WithPasswordHash("A").WithLogin("C")
             .WithLogin("ab")
             .Build();

        m_users = [user1, user2, user3];

        // Act
        var found = await m_userService.FindUser("d");

        // Assert
        Assert.Empty(found);
    }

    [Fact]
    public async Task GetUser_Ok()
    {
        // Arrange
        var user = new UserBuilder()
            .WithID(Guid.NewGuid())
            .WithPasswordHash("A")
            .WithLogin("A")
            .Build();

        m_users = [user];

        // Act
        var found = await m_userService.GetUser(user.ID);

        // Assert
        Assert.Equivalent(found, user);
    }

    [Fact]
    public async Task GetUser_NotFound()
    {
        // Arrange
        var user = new UserBuilder().WithID(Guid.NewGuid()).WithPasswordHash("A").WithLogin("A")
            .WithID(new Guid())
            .Build();

        m_users = [];

        // Act
        var action = async () => await m_userService.GetUser(new Guid());

        // Assert
        await Assert.ThrowsAsync<UserNotFoundException>(action);
    }

    [Fact]
    public async Task ListAllUsers_Ok()
    {
        // Arrange
        var user1 = new UserBuilder().WithID(Guid.NewGuid()).WithPasswordHash("A").WithLogin("A")
            .WithRole(Role.User)
            .Build();

        var user2 = new UserBuilder().WithID(Guid.NewGuid()).WithPasswordHash("A").WithLogin("B")
            .WithRole(Role.User)
            .Build();

        var admin1 = new UserBuilder().WithID(Guid.NewGuid()).WithPasswordHash("A").WithLogin("C")
            .WithRole(Role.User)
            .Build();

        var admin2 = new UserBuilder().WithID(Guid.NewGuid()).WithPasswordHash("A").WithLogin("D")
            .WithRole(Role.User)
            .Build();

        m_users = [user1, user2, admin1, admin2];

        // Act
        var result = await m_userService.ListAllUsers();

        // Assert
        Assert.Equivalent(result, m_users);
    }

    [Fact]
    public async Task ListAllUsers_Empty()
    {
        // Arrange

        // Act
        var result = await m_userService.ListAllUsers();

        // Assert
        Assert.Empty(result);
    }
}
