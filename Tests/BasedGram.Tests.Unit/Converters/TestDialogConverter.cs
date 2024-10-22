using BasedGram.Common.Core;
using BasedGram.Database.Npgsql.Models;
using BasedGram.Database.Npgsql.Models.Converters;

namespace BasedGram.Tests.Unit.Converters;

public class TestDialogConverter
{
    [Fact]
    public void DbToCoreModel_ShouldConvertProperly()
    {
        // Arrange
        var dbModel = new DialogDbModel
        {
            ID = Guid.NewGuid(),
            IsBlockedFlag = true,
            CreatorID = Guid.NewGuid(),
            ColocutorID = Guid.NewGuid()
        };

        // Act
        var coreModel = DialogConverter.DbToCoreModel(dbModel);

        // Assert
        Assert.NotNull(coreModel);
        Assert.Equal(dbModel.ID, coreModel.ID);
        Assert.Equal(dbModel.IsBlockedFlag, coreModel.IsBlockedFlag);
        Assert.Equal(dbModel.CreatorID, coreModel.CreatorID);
        Assert.Equal(dbModel.ColocutorID, coreModel.TargetID);
    }

    [Fact]
    public void DbToCoreModel_ShouldReturnNull_WhenNullInput()
    {
        // Act
        var coreModel = DialogConverter.DbToCoreModel(null);

        // Assert
        Assert.Null(coreModel);
    }

    [Fact]
    public void CoreToDbModel_ShouldConvertProperly()
    {
        // Arrange
        var coreModel = new Dialog
        {
            ID = Guid.NewGuid(),
            IsBlockedFlag = false,
            CreatorID = Guid.NewGuid(),
            TargetID = Guid.NewGuid()
        };

        // Act
        var dbModel = DialogConverter.CoreToDbModel(coreModel);

        // Assert
        Assert.NotNull(dbModel);
        Assert.Equal(coreModel.ID, dbModel.ID);
        Assert.Equal(coreModel.IsBlockedFlag, dbModel.IsBlockedFlag);
        Assert.Equal(coreModel.CreatorID, dbModel.CreatorID);
        Assert.Equal(coreModel.TargetID, dbModel.ColocutorID);
    }

    [Fact]
    public void CoreToDbModel_ShouldReturnNull_WhenNullInput()
    {
        // Act
        var dbModel = DialogConverter.CoreToDbModel(null);

        // Assert
        Assert.Null(dbModel);
    }
}