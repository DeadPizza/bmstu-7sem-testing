using BasedGram.Common.Core;
using BasedGram.Common.Enums;
using BasedGram.Database.Npgsql.Models;
using BasedGram.Database.Npgsql.Models.Converters;

namespace BasedGram.Tests.Unit.Converters;

public class TestUserConverter
{
    [Fact]
    public void DbToCoreModel_ShouldConvertProperly()
    {
        // Arrange
        var dbModel = new UserDbModel
        {
            ID = Guid.NewGuid(),
            Login = "testuser",
            PasswordHash = "hashedPassword",
            Role = 0,
            IsAuthorised = true,
            IsFreezed = false
        };

        // Act
        var coreModel = UserConverter.DbToCoreModel(dbModel);

        // Assert
        Assert.NotNull(coreModel);
        Assert.Equal(dbModel.ID, coreModel.ID);
        Assert.Equal(dbModel.Login, coreModel.Login);
        Assert.Equal(dbModel.PasswordHash, coreModel.PasswordHash);
        Assert.Equal(Role.User, coreModel.Role);
        Assert.Equal(dbModel.IsAuthorised, coreModel.AuthFlag);
        Assert.Equal(dbModel.IsFreezed, coreModel.IsFreezed);
    }

    [Fact]
    public void DbToCoreModel_ShouldReturnNull_WhenNullInput()
    {
        // Act
        var coreModel = UserConverter.DbToCoreModel(null);

        // Assert
        Assert.Null(coreModel);
    }

    [Fact]
    public void CoreToDbModel_ShouldConvertProperly()
    {
        // Arrange
        var coreModel = new User
        {
            ID = Guid.NewGuid(),
            Login = "testuser",
            PasswordHash = "hashedPassword",
            Role = Role.Admin,
            AuthFlag = true,
            IsFreezed = false
        };

        // Act
        var dbModel = UserConverter.CoreToDbModel(coreModel);

        // Assert
        Assert.NotNull(dbModel);
        Assert.Equal(coreModel.ID, dbModel.ID);
        Assert.Equal(coreModel.Login, dbModel.Login);
        Assert.Equal(coreModel.PasswordHash, dbModel.PasswordHash);
        Assert.Equal(1, dbModel.Role);
        Assert.Equal(coreModel.AuthFlag, dbModel.IsAuthorised);
        Assert.Equal(coreModel.IsFreezed, dbModel.IsFreezed);
    }

    [Fact]
    public void CoreToDbModel_ShouldReturnNull_WhenNullInput()
    {
        // Act
        var dbModel = UserConverter.CoreToDbModel(null);

        // Assert
        Assert.Null(dbModel);
    }
}
