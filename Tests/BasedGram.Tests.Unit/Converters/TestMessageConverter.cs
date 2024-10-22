using BasedGram.Common.Core;
using BasedGram.Database.Npgsql.Models;
using BasedGram.Database.Npgsql.Models.Converters;

namespace BasedGram.Tests.Unit.Converters;

public class TestMessageConverter
{
    [Fact]
    public void DbToCoreModel_ShouldConvertProperly()
    {
        // Arrange
        var dbModel = new MessageDbModel
        {
            ID = Guid.NewGuid(),
            SenderID = Guid.NewGuid(),
            DialogID = Guid.NewGuid(),
            SentTime = DateTime.UtcNow,
            Content = "Test message",
            isReadFlag = true,
            ReactionState = 1,
            Embedding = ""
        };

        // Act
        var coreModel = MessageConverter.DbToCoreModel(dbModel);

        // Assert
        Assert.NotNull(coreModel);
        Assert.Equal(dbModel.ID, coreModel.ID);
        Assert.Equal(dbModel.SenderID, coreModel.SenderID);
        Assert.Equal(dbModel.DialogID, coreModel.DialogID);
        Assert.Equal(dbModel.SentTime, coreModel.SendTime);
        Assert.Equal(dbModel.Content, coreModel.Content);
        Assert.Equal(dbModel.isReadFlag, coreModel.isReadFlag);
        Assert.Equal(dbModel.ReactionState, (int)coreModel.ReactionState);
        Assert.Equal(dbModel.Embedding, coreModel.Embedding);
    }

    [Fact]
    public void DbToCoreModel_ShouldReturnNull_WhenNullInput()
    {
        // Act
        var coreModel = MessageConverter.DbToCoreModel(null);

        // Assert
        Assert.Null(coreModel);
    }

    [Fact]
    public void CoreToDbModel_ShouldConvertProperly()
    {
        // Arrange
        var coreModel = new Message
        {
            ID = Guid.NewGuid(),
            SenderID = Guid.NewGuid(),
            DialogID = Guid.NewGuid(),
            SendTime = DateTime.UtcNow,
            Content = "Test message",
            isReadFlag = false,
            ReactionState = 0,
            Embedding = ""
        };

        // Act
        var dbModel = MessageConverter.CoreToDbModel(coreModel);

        // Assert
        Assert.NotNull(dbModel);
        Assert.Equal(coreModel.ID, dbModel.ID);
        Assert.Equal(coreModel.SenderID, dbModel.SenderID);
        Assert.Equal(coreModel.DialogID, dbModel.DialogID);
        Assert.Equal(coreModel.SendTime.ToUniversalTime(), dbModel.SentTime);
        Assert.Equal(coreModel.Content, dbModel.Content);
        Assert.Equal(coreModel.isReadFlag, dbModel.isReadFlag);
        Assert.Equal(0, dbModel.ReactionState);
        Assert.Equal(coreModel.Embedding, dbModel.Embedding);
    }

    [Fact]
    public void CoreToDbModel_ShouldReturnNull_WhenNullInput()
    {
        // Act
        var dbModel = MessageConverter.CoreToDbModel(null);

        // Assert
        Assert.Null(dbModel);
    }
}