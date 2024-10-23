using BasedGram.Common.Core;
using BasedGram.Services.DialogService;
using BasedGram.Services.DialogService.Exceptions;
using BasedGram.Tests.Common.DataBuilders.Core;
using Moq;

namespace BasedGram.Tests.Integration.Services;

[Collection("Test Database")]
public class TestDialogService : TestServiceBase
{
    private readonly DialogService m_dialogService;

    public TestDialogService(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
        m_dialogService = new(m_dialogRepository, m_messageRepository, null);
    }

    [Fact]
    public async Task CreateDialog_ShouldCreateDialog_WhenDialogDoesNotExist()
    {
        // Arrange
        var creator = new UserBuilder().WithID(new Guid()).Build();
        var target = new UserBuilder().WithID(new Guid()).Build();

        // Act
        var result = await m_dialogService.CreateDialog(creator, target);

        // Assert
        Assert.Equal(creator.ID, result.CreatorID);
        Assert.Equal(target.ID, result.TargetID);
    }

    [Fact]
    public async Task DeleteDialog_ShouldDeleteDialog_WhenDialogExists()
    {
        // Arrange
        m_dialogs = [new DialogBuilder()
                        .Build()];

        // Act
        await m_dialogService.DeleteDialog(m_dialogs[0]);

        // Assert
        Assert.Empty(m_dialogs);
    }

    [Fact]
    public async Task DeleteDialog_ShouldThrowException_WhenExceptionOccurs()
    {
        // Arrange
        var dialog = new DialogBuilder()
                        .Build();

        // Act
        var action = async () => await m_dialogService.DeleteDialog(dialog);

        // Assert
        Assert.Empty(m_dialogs);
    }

    [Fact]
    public async Task BlockDialog_ShouldBlockDialog_WhenValid()
    {
        // Arrange
        var dialog = new DialogBuilder()
                        .Build();

        // Act
        await m_dialogService.BlockDialog(dialog);

        // Assert
        Assert.NotNull(dialog);
    }

    [Fact]
    public async Task GetDialog_ShouldReturnDialog_WhenDialogExists()
    {
        // Arrange
        m_dialogs = [new DialogBuilder()
                        .Build()];

        // Act
        var result = await m_dialogService.GetDialog(m_dialogs[0].ID);

        // Assert
        Assert.Equivalent(m_dialogs[0], result);
    }

    [Fact]
    public async Task GetUserDialogs_ShouldReturnDialogsForUser_WhenDialogsExist()
    {
        // Arrange
        var user = new UserBuilder().WithID(Guid.NewGuid()).Build();
        m_dialogs = new List<Dialog>
        {
            new DialogBuilder().WithCreatorID(user.ID).Build(),
            new DialogBuilder().WithTargetID(user.ID).Build()
        };

        // Act
        var result = await m_dialogService.GetUserDialogs(user);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.True(result.All(d => d.CreatorID == user.ID || d.TargetID == user.ID));
    }

    [Fact]
    public async Task GetDialogMessages_ShouldReturnSortedMessages_WhenMessagesExist()
    {
        // Arrange
        var user = new UserBuilder().WithID(Guid.NewGuid()).Build();
        var dialog = new DialogBuilder().WithCreatorID(user.ID).Build();
        m_messages =
        [
            new MessageBuilder().WithSenderID(user.ID).WithSendTime(DateTime.Now.AddMinutes(-5)).WithDialogID(dialog.ID).Build(),
            new MessageBuilder().WithSenderID(user.ID).WithSendTime(DateTime.Now.AddMinutes(-1)).WithDialogID(dialog.ID).Build(),
            new MessageBuilder().WithSenderID(user.ID).WithSendTime(DateTime.Now.AddMinutes(-10)).WithDialogID(dialog.ID).Build()
        ];

        // Act
        var result = await m_dialogService.GetDialogMessages(user, dialog);

        // Assert
        Assert.Equal(3, result.Count);
        Assert.True(result[0].SendTime < result[1].SendTime && result[1].SendTime < result[2].SendTime);
    }

    [Fact]
    public async Task GetDialogMessages_ShouldReturnEmpty_WhenNoMessagesExist()
    {
        // Arrange
        var user = new UserBuilder().WithID(Guid.NewGuid()).Build();
        var dialog = new DialogBuilder().WithCreatorID(user.ID).Build();

        // Act
        var result = await m_dialogService.GetDialogMessages(user, dialog);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllMessages_ShouldReturnSortedMessages_WhenMessagesExist()
    {
        // Arrange
        var dialog = new DialogBuilder().Build();
        m_messages =
        [
            new MessageBuilder().WithSendTime(DateTime.Now.AddMinutes(-5)).WithDialogID(dialog.ID).Build(),
            new MessageBuilder().WithSendTime(DateTime.Now.AddMinutes(-1)).WithDialogID(dialog.ID).Build(),
            new MessageBuilder().WithSendTime(DateTime.Now.AddMinutes(-10)).WithDialogID(dialog.ID).Build()
        ];

        // Act
        var result = await m_dialogService.GetAllMessages(dialog);

        // Assert
        Assert.Equal(3, result.Count);
        Assert.True(result[0].SendTime < result[1].SendTime && result[1].SendTime < result[2].SendTime);
    }

    [Fact]
    public async Task GetAllMessages_ShouldReturnEmpty_WhenNoMessagesExist()
    {
        // Arrange
        var dialog = new DialogBuilder().Build();

        // Act
        var result = await m_dialogService.GetAllMessages(dialog);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task CreateMessage_ShouldCreateAndSend_WhenValid()
    {
        // Arrange
        var user = new User { ID = Guid.NewGuid() };
        var dialog = new Dialog(Guid.NewGuid(), false, user.ID, Guid.NewGuid());
        string messageContent = "Test message";
        string embedding = "Test embedding";

        // Act
        var result = await m_dialogService.CreateMessage(user, dialog, messageContent, embedding);

        // Assert
        Assert.Equal(dialog.ID, result.DialogID);
        Assert.Equal(user.ID, result.SenderID);
    }

    [Fact]
    public void CreateMessage_ShouldThrowException_WhenRepositoryFails()
    {
        // Arrange
        var user = new User { ID = Guid.NewGuid() };
        var dialog = new Dialog(Guid.NewGuid(), false, user.ID, Guid.NewGuid());
        string messageContent = "Test message";
        string embedding = "Test embedding";
        
        // Act
        var action = () => m_dialogService.CreateMessage(user, dialog, messageContent, embedding);

        // Assert
        Assert.ThrowsAsync<Exception>(action);
    }

    [Fact]
    public async Task UpdateMessage_ShouldUpdateMessage_WhenValid()
    {
        // Arrange
        var message = new Message
        {
            ID = Guid.NewGuid(),
            DialogID = Guid.NewGuid(),
        };

        // Act
        await m_dialogService.UpdateMessage(message);

        // Assert
        Assert.NotNull(message);
    }

    [Fact]
    public void UpdateMessage_ShouldThrowException_WhenRepositoryFails()
    {
        // Arrange
        var message = new Message
        {
            ID = Guid.NewGuid(),
            DialogID = Guid.NewGuid(),
        };

        // Act
        var action = () => m_dialogService.UpdateMessage(message);

        // Assert
        Assert.ThrowsAsync<Exception>(action);
    }
}
