using BasedGram.Database.Npgsql.Models.Converters;
using BasedGram.Database.Npgsql.Models;
using BasedGram.Database.NpgsqlRepositories;
using BasedGram.Tests.Common.Factories.Core;
using BasedGram.Tests.Common.Factories.Db;
using Moq;
using Moq.EntityFrameworkCore;
using BasedGram.Common.Core;
using Microsoft.EntityFrameworkCore;
using Abp.Extensions;

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

        [Fact]
        public async Task CreateMessage_Ok()
        {
            // Arrange
            var users = await CreateDefaultUsers();
            var message = MessageDbModelFactory.Create(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                DateTime.Now.ToUniversalTime(),
                "Test Content",
                false, 0, "");

            // Act
            await m_messageRepository.CreateMessage(MessageConverter.DbToCoreModel(message));

            // Assert
            var models = (from a in m_dbFixture.CreateContext().Messages select a).ToList();
            Assert.Single(models);
        }

        [Fact]
        public async Task CreateMessage_ExceptionThrown()
        {
            // Arrange
            var users = await CreateDefaultUsers();
            var message = MessageDbModelFactory.Create(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                DateTime.Now.ToUniversalTime(),
                "Test Content",
                false, 0, "");
            await CreateSingleMessageFrom(message);

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateException>(() => m_messageRepository.CreateMessage(MessageConverter.DbToCoreModel(message)));
        }

        [Fact]
        public async Task DeleteMessage_Ok()
        {
            // Arrange
            var users = await CreateDefaultUsers();
            var message = MessageDbModelFactory.Create(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                DateTime.Now.ToUniversalTime(),
                "Test Content",
                false, 0, "");
            await CreateSingleMessageFrom(message);

            // Act
            await m_messageRepository.DeleteMessage(MessageConverter.DbToCoreModel(message));

            // Assert
            var models = (from a in m_dbFixture.CreateContext().Messages select a).ToList();
            Assert.Empty(models);
        }

        [Fact]
        public async Task DeleteMessage_NotFound()
        {
            // Arrange
            var users = await CreateDefaultUsers();
            var message = MessageDbModelFactory.Create(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                DateTime.Now.ToUniversalTime(),
                "Test Content",
                false, 0, "");

            // Act
            await m_messageRepository.DeleteMessage(MessageConverter.DbToCoreModel(message));

            // Assert
            var models = (from a in m_dbFixture.CreateContext().Messages select a).ToList();
            Assert.Empty(models);
        }

        [Fact]
        public async Task GetMessageByID_Ok()
        {
            // Arrange
            var message = MessageDbModelFactory.Create(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                DateTime.Now.ToUniversalTime(),
                "Test Content",
                false, 0, "");
            await CreateSingleMessageFrom(message);

            // Act
            var result = await m_messageRepository.GetMessageByID(message.ID);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetMessageByID_NotFound()
        {
            // Arrange
            var messageID = Guid.NewGuid();

            // Act
            var result = await m_messageRepository.GetMessageByID(messageID);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateMessage_Ok()
        {
            // Arrange
            var users = await CreateDefaultUsers();
            var message = MessageDbModelFactory.Create(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                DateTime.Now.ToUniversalTime(),
                "Test Content",
                false, 0, "");
            await CreateSingleMessageFrom(message);

            var updatedMessage = MessageDbModelFactory.Create(
                message.ID,
                Guid.NewGuid(),
                Guid.NewGuid(),
                DateTime.Now,
                "New Content",
                false, 0, "");

            // Act
            await m_messageRepository.UpdateMessage(MessageConverter.DbToCoreModel(updatedMessage));

            // Assert
            var models = (from a in m_dbFixture.CreateContext().Messages select a).ToList();
            Assert.Equal(updatedMessage.Content, models[0].Content);
            Assert.Equal(updatedMessage.isReadFlag, models[0].isReadFlag);
        }

        [Fact]
        public async Task UpdateMessage_NotFound()
        {
            // Arrange
            var message = MessageDbModelFactory.Create(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                DateTime.Now.ToUniversalTime(),
                "Test Content",
                false, 0, "");

            // Act
            await m_messageRepository.UpdateMessage(MessageConverter.DbToCoreModel(message));

            // Assert
            var models = (from a in m_dbFixture.CreateContext().Messages select a).ToList();
            Assert.Single(models);
        }

        [Fact]
        public async Task GetUserMessagesInDialog_Ok()
        {
            // Arrange
            var users = await CreateDefaultUsers();
            var dialog = DialogDbModelFactory.Create(
                Guid.NewGuid(),
                false,
                users[0].ID,
                users[1].ID);
            await CreateSingleDialogFrom(dialog);

            var message = MessageDbModelFactory.Create(
                Guid.NewGuid(),
                users[0].ID,
                dialog.ID,
                DateTime.Now.ToUniversalTime(),
                "Test Content",
                false, 0, "");
            await CreateSingleMessageFrom(message);

            // Act
            var result = await m_messageRepository.GetUserMessagesInDialog(
                UserConverter.DbToCoreModel(users[0]),
                DialogConverter.DbToCoreModel(dialog));

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task GetUserMessagesInDialog_NotFound()
        {
            // Arrange
            var user = UserFactory.CreateEmpty();
            var dialog = DialogFactory.CreateEmpty();

            // Act
            var result = await m_messageRepository.GetUserMessagesInDialog(user, dialog);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllMessagesInDialog_Ok()
        {
            // Arrange
            var users = await CreateDefaultUsers();
            var dialog = DialogDbModelFactory.Create(
                Guid.NewGuid(),
                false,
                users[0].ID,
                users[1].ID);
            await CreateSingleDialogFrom(dialog);

            var message = MessageDbModelFactory.Create(
                Guid.NewGuid(),
                users[0].ID,
                dialog.ID,
                DateTime.Now.ToUniversalTime(),
                "Test Content",
                false, 0, "");
            await CreateSingleMessageFrom(message);

            // Act
            var result = await m_messageRepository.GetAllMessagesInDialog(DialogConverter.DbToCoreModel(dialog));

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task GetAllMessagesInDialog_NotFound()
        {
            // Arrange
            var dialog = DialogFactory.CreateEmpty();

            // Act
            var result = await m_messageRepository.GetAllMessagesInDialog(dialog);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllMessages_Ok()
        {
            // Arrange
            var users = await CreateDefaultUsers();
            var dialog = DialogDbModelFactory.Create(
                Guid.NewGuid(),
                false,
                users[0].ID,
                users[1].ID);
            await CreateSingleDialogFrom(dialog);

            var message = MessageDbModelFactory.Create(
                Guid.NewGuid(),
                users[0].ID,
                dialog.ID,
                DateTime.Now.ToUniversalTime(),
                "Test Content",
                false, 0, "");
            await CreateSingleMessageFrom(message);

            // Act
            var result = await m_messageRepository.GetAllMessages();

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task GetAllMessages_Empty()
        {
            // Arrange

            // Act
            var result = await m_messageRepository.GetAllMessages();

            // Assert
            Assert.Empty(result);
        }
    }
}