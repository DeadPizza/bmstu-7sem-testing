using BasedGram.Database.Npgsql.Models.Converters;
using BasedGram.Database.Npgsql.Models;
using BasedGram.Database.NpgsqlRepositories;
using BasedGram.Tests.Common.Factories.Core;
using BasedGram.Tests.Common.Factories.Db;
using Moq;
using Moq.EntityFrameworkCore;
using BasedGram.Common.Core;

namespace BasedGram.Tests.Integration.Repositories
{
    [Collection("Test Database")]
    public class TestMessageRepository : TestRepositoryBase
    {
        private readonly MessageRepository m_messageRepository;

        public TestMessageRepository(DatabaseFixture fixture) : base(fixture)
        {
            m_messageRepository = new MessageRepository(m_dbFixture.CreateContext());
        }

        // [Fact]
        // public async Task CreateMessage_Ok()
        // {
        //     // Arrange
        //     List<MessageDbModel> models = new List<MessageDbModel>();
        //     var message = MessageDbModelFactory.Create(
        //         Guid.NewGuid(),
        //         Guid.NewGuid(),
        //         Guid.NewGuid(),
        //         DateTime.Now.ToUniversalTime(),
        //         "Test Content",
        //         false, 0, "");

        //     m_mockDbContextFactory
        //         .MockMessages
        //         .Setup(s => s.AddAsync(It.IsAny<MessageDbModel>(), It.IsAny<CancellationToken>()))
        //         .Callback<MessageDbModel, CancellationToken>((a, token) => models.Add(a));

        //     // Act
        //     await m_messageRepository.CreateMessage(MessageConverter.DbToCoreModel(message));

        //     // Assert
        //     Assert.Single(models);
        //     Assert.Equivalent(message, models[0]);
        // }

        // [Fact]
        // public async Task CreateMessage_ExceptionThrown()
        // {
        //     // Arrange
        //     var message = MessageDbModelFactory.Create(
        //         Guid.NewGuid(),
        //         Guid.NewGuid(),
        //         Guid.NewGuid(),
        //         DateTime.Now,
        //         "Test Content",
        //         false, 0, "");

        //     m_mockDbContextFactory
        //         .MockMessages
        //         .Setup(s => s.AddAsync(It.IsAny<MessageDbModel>(), It.IsAny<CancellationToken>()))
        //         .ThrowsAsync(new Exception("Database Error"));

        //     // Act & Assert
        //     await Assert.ThrowsAsync<Exception>(() => m_messageRepository.CreateMessage(MessageConverter.DbToCoreModel(message)));
        // }

        // [Fact]
        // public async Task DeleteMessage_Ok()
        // {
        //     // Arrange
        //     var message = MessageDbModelFactory.Create(
        //         Guid.NewGuid(),
        //         Guid.NewGuid(),
        //         Guid.NewGuid(),
        //         DateTime.Now,
        //         "Test Content",
        //         false, 0, "");

        //     List<MessageDbModel> models = [message];

        //     m_mockDbContextFactory
        //         .MockMessages
        //         .Setup(s => s.FindAsync(It.IsAny<Guid>()))
        //         .ReturnsAsync(message);

        //     m_mockDbContextFactory
        //         .MockMessages
        //         .Setup(s => s.Remove(It.IsAny<MessageDbModel>()))
        //         .Callback((MessageDbModel m) => models.Remove(m));

        //     // Act
        //     await m_messageRepository.DeleteMessage(MessageConverter.DbToCoreModel(message));

        //     // Assert
        //     Assert.Empty(models);
        // }

        // [Fact]
        // public async Task DeleteMessage_NotFound()
        // {
        //     // Arrange
        //     var message = MessageDbModelFactory.Create(
        //         Guid.NewGuid(),
        //         Guid.NewGuid(),
        //         Guid.NewGuid(),
        //         DateTime.Now,
        //         "Test Content",
        //         false, 0, "");

        //     m_mockDbContextFactory
        //         .MockMessages
        //         .Setup(s => s.FindAsync(It.IsAny<Guid>()))
        //         .ReturnsAsync((MessageDbModel)null);

        //     // Act
        //     await m_messageRepository.DeleteMessage(MessageConverter.DbToCoreModel(message));

        //     // Assert
        //     // No exception should be thrown
        //     m_mockDbContextFactory.MockMessages.Verify(s => s.Remove(It.IsAny<MessageDbModel>()), Times.Never);
        // }

        // [Fact]
        // public async Task GetMessageByID_Ok()
        // {
        //     // Arrange
        //     var message = MessageDbModelFactory.Create(
        //         Guid.NewGuid(),
        //         Guid.NewGuid(),
        //         Guid.NewGuid(),
        //         DateTime.Now,
        //         "Test Content",
        //         false, 0, "");

        //     m_mockDbContextFactory
        //         .MockMessages
        //         .Setup(s => s.FindAsync(It.IsAny<Guid>()))
        //         .ReturnsAsync(message);

        //     // Act
        //     var result = await m_messageRepository.GetMessageByID(message.ID);

        //     // Assert
        //     Assert.NotNull(result);
        //     Assert.Equivalent(MessageConverter.DbToCoreModel(message), result);
        // }

        // [Fact]
        // public async Task GetMessageByID_NotFound()
        // {
        //     // Arrange
        //     var messageID = Guid.NewGuid();

        //     m_mockDbContextFactory
        //         .MockMessages
        //         .Setup(s => s.FindAsync(It.IsAny<Guid>()))
        //         .ReturnsAsync((MessageDbModel)null);

        //     // Act
        //     var result = await m_messageRepository.GetMessageByID(messageID);

        //     // Assert
        //     Assert.Null(result);
        // }

        // [Fact]
        // public async Task UpdateMessage_Ok()
        // {
        //     // Arrange
        //     var existingMessage = MessageDbModelFactory.Create(
        //         Guid.NewGuid(),
        //         Guid.NewGuid(),
        //         Guid.NewGuid(),
        //         DateTime.Now,
        //         "Old Content",
        //         false, 0, "");

        //     var updatedMessage = MessageDbModelFactory.Create(
        //         Guid.NewGuid(),
        //         Guid.NewGuid(),
        //         Guid.NewGuid(),
        //         DateTime.Now,
        //         "New Content",
        //         false, 0, "");

        //     m_mockDbContextFactory
        //         .MockMessages
        //         .Setup(s => s.FindAsync(It.IsAny<Guid>()))
        //         .ReturnsAsync(existingMessage);

        //     // Act
        //     await m_messageRepository.UpdateMessage(MessageConverter.DbToCoreModel(updatedMessage));

        //     // Assert
        //     Assert.Equal(updatedMessage.Content, existingMessage.Content);
        //     Assert.Equal(updatedMessage.isReadFlag, existingMessage.isReadFlag);
        // }

        // [Fact]
        // public async Task UpdateMessage_NotFound()
        // {
        //     // Arrange
        //     var message = MessageDbModelFactory.Create(
        //         Guid.NewGuid(),
        //         Guid.NewGuid(),
        //         Guid.NewGuid(),
        //         DateTime.Now,
        //         "Test Content",
        //         false, 0, "");

        //     m_mockDbContextFactory
        //         .MockMessages
        //         .Setup(s => s.FindAsync(It.IsAny<Guid>()))
        //         .ReturnsAsync((MessageDbModel)null);

        //     // Act
        //     await m_messageRepository.UpdateMessage(MessageConverter.DbToCoreModel(message));

        //     // Assert
        //     m_mockDbContextFactory.MockMessages.Verify(s => s.AddAsync(It.IsAny<MessageDbModel>(), It.IsAny<CancellationToken>()), Times.Once);
        // }

        // [Fact]
        // public async Task GetUserMessagesInDialog_Ok()
        // {
        //     // Arrange
        //     var user = UserFactory.CreateEmpty();
        //     var dialog = DialogFactory.CreateEmpty();

        //     var message = MessageDbModelFactory.Create(
        //         Guid.NewGuid(),
        //         user.ID,
        //         dialog.ID,
        //         DateTime.Now,
        //         "Test Content",
        //         false, 0, "");

        //     List<MessageDbModel> messages = new List<MessageDbModel> { message };

        //     m_mockDbContextFactory
        //         .MockContext
        //             .Setup(x => x.Messages)
        //             .ReturnsDbSet(messages);

        //     // Act
        //     var result = await m_messageRepository.GetUserMessagesInDialog(user, dialog);

        //     // Assert
        //     Assert.Single(result);
        //     Assert.Equivalent(MessageConverter.DbToCoreModel(message), result[0]);
        // }

        // [Fact]
        // public async Task GetUserMessagesInDialog_NotFound()
        // {
        //     // Arrange
        //     var user = UserFactory.CreateEmpty();
        //     var dialog = DialogFactory.CreateEmpty();

        //     List<MessageDbModel> messages = [];

        //     m_mockDbContextFactory
        //         .MockContext
        //             .Setup(x => x.Messages)
        //             .ReturnsDbSet(messages);

        //     // Act
        //     var result = await m_messageRepository.GetUserMessagesInDialog(user, dialog);

        //     // Assert
        //     Assert.Empty(result);
        // }

        // [Fact]
        // public async Task GetAllMessagesInDialog_Ok()
        // {
        //     // Arrange
        //     var dialog = DialogFactory.CreateEmpty();

        //     var message = MessageDbModelFactory.Create(
        //         Guid.NewGuid(),
        //         Guid.NewGuid(),
        //         dialog.ID,
        //         DateTime.Now,
        //         "Test Content",
        //         false, 0, "");

        //     List<MessageDbModel> messages = new List<MessageDbModel> { message };

        //     m_mockDbContextFactory
        //         .MockContext
        //             .Setup(x => x.Messages)
        //             .ReturnsDbSet(messages);

        //     // Act
        //     var result = await m_messageRepository.GetAllMessagesInDialog(dialog);

        //     // Assert
        //     Assert.Single(result);
        //     Assert.Equivalent(MessageConverter.DbToCoreModel(message), result[0]);
        // }

        // [Fact]
        // public async Task GetAllMessagesInDialog_NotFound()
        // {
        //     // Arrange
        //     var dialog = DialogFactory.CreateEmpty();
        //     List<MessageDbModel> messages = new List<MessageDbModel> { };

        //     m_mockDbContextFactory
        //         .MockContext
        //             .Setup(x => x.Messages)
        //             .ReturnsDbSet(messages);


        //     // Act
        //     var result = await m_messageRepository.GetAllMessagesInDialog(dialog);

        //     // Assert
        //     Assert.Empty(result);
        // }

        // [Fact]
        // public async Task GetAllMessages_Ok()
        // {
        //     // Arrange
        //     var message = MessageDbModelFactory.Create(
        //         Guid.NewGuid(),
        //         Guid.NewGuid(),
        //         Guid.NewGuid(),
        //         DateTime.Now,
        //         "Test Content",
        //         false, 0, "");

        //     List<MessageDbModel> messages = new List<MessageDbModel> { message };

        //     m_mockDbContextFactory
        //         .MockMessages
        //         .Setup(s => s.ToListAsync(It.IsAny<CancellationToken>()))
        //         .ReturnsAsync(messages);

        //     // Act
        //     var result = await m_messageRepository.GetAllMessages();

        //     // Assert
        //     Assert.Single(result);
        //     Assert.Equal(MessageConverter.DbToCoreModel(message), result[0]);
        // }

        // [Fact]
        // public async Task GetAllMessages_Empty()
        // {
        //     // Arrange
        //     m_mockDbContextFactory
        //         .MockMessages
        //         .Setup(s => s.ToListAsync(It.IsAny<CancellationToken>()))
        //         .ReturnsAsync(new List<MessageDbModel>());

        //     // Act
        //     var result = await m_messageRepository.GetAllMessages();

        //     // Assert
        //     Assert.Empty(result);
        // }
    }
}