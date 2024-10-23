using BasedGram.Database.Npgsql.Models.Converters;
using BasedGram.Database.Npgsql.Models;
using BasedGram.Database.NpgsqlRepositories;
using BasedGram.Tests.Common.Factories.Core;
using BasedGram.Tests.Common.Factories.Db;
using Moq;
using BasedGram.Common.Core;
using Microsoft.EntityFrameworkCore;
using BasedGram.Tests.Common.DataBuilders.Db;
using BasedGram.Tests.Common.DataBuilders.Core;

namespace BasedGram.Tests.Integration.Repositories;

[Collection("Test Database")]
public class TestDialogRepository : TestRepositoryBase
{
    protected readonly DialogRepository m_dialogRepository;
    public TestDialogRepository(DatabaseFixture fixture) : base(fixture)
    {
        m_dialogRepository = new(m_dbFixture.CreateContext());
    }

    [Fact]
    public async Task CreateDialog_Ok()
    {
        // Arrange
        var users = await CreateDefaultUsers();
        var dialog = DialogDbModelFactory.Create(
            Guid.NewGuid(),
            false,
            users[0].ID,
            users[1].ID);

        // Act
        await m_dialogRepository.CreateDialog(DialogConverter.DbToCoreModel(dialog));

        // Assert
        var models = (from a in m_dbFixture.CreateContext().Dialogs select a).ToList();
        Assert.Single(models);
        Assert.Equivalent(dialog, models[0]);
    }

    [Fact]
    public async Task CreateDialog_NonUniqueException()
    {
        // Arrange
        var users = await CreateDefaultUsers();
        var dialog = DialogDbModelFactory.Create(
            Guid.NewGuid(),
            false,
            users[0].ID,
            users[1].ID);
        await CreateSingleDialogFrom(dialog);

        // Act
        async Task action() => await m_dialogRepository.CreateDialog(DialogConverter.DbToCoreModel(dialog));

        // Assert
        await Assert.ThrowsAsync<DbUpdateException>(action);
    }

    [Fact]
    public async Task DeleteDialog_Ok()
    {
        // Arrange
        var users = await CreateDefaultUsers();
        var dialog = DialogDbModelFactory.Create(
            Guid.NewGuid(),
            false,
            users[0].ID,
            users[1].ID);
        await CreateSingleDialogFrom(dialog);


        // Act
        await m_dialogRepository.DeleteDialog(DialogConverter.DbToCoreModel(dialog));

        // Assert
        Assert.Empty((from a in m_dbFixture.CreateContext().Dialogs select a).ToList());
    }

    [Fact]
    public async Task DeleteDialog_NotFound()
    {
        // Arrange
        var users = await CreateDefaultUsers();
        var dialog = DialogDbModelFactory.Create(
            Guid.NewGuid(),
            false,
            users[0].ID,
            users[1].ID);
        List<DialogDbModel> models = [dialog];

        // Act
        await m_dialogRepository.DeleteDialog(DialogConverter.DbToCoreModel(dialog));

        // Assert
        Assert.Equivalent(models, new List<DialogDbModel>([dialog]));
    }

    [Fact]
    public async Task BlockDialog_Ok()
    {
        // Arrange
        var users = await CreateDefaultUsers();
        var dialog = DialogDbModelFactory.Create(
            Guid.NewGuid(),
            false,
            users[0].ID,
            users[1].ID);
        await CreateSingleDialogFrom(dialog);

        // Act
        await m_dialogRepository.BlockDialog(DialogConverter.DbToCoreModel(dialog));

        // Assert
        var models = (from a in m_dbFixture.CreateContext().Dialogs select a).ToList();
        Assert.True(models[0].IsBlockedFlag);
    }

    [Fact]
    public async Task BlockDialog_NotFound()
    {
        // Arrange
        var users = await CreateDefaultUsers();
        var dialog = DialogDbModelFactory.Create(
            Guid.NewGuid(),
            false,
            users[0].ID,
            users[1].ID);
        await CreateSingleDialogFrom(dialog);

        // Act
        await m_dialogRepository.BlockDialog(new DialogBuilder().Build());

        // Assert
        var models = (from a in m_dbFixture.CreateContext().Dialogs select a).ToList();
        Assert.False(models[0].IsBlockedFlag);
    }

    [Fact]
    public async Task UnBlockDialog_Ok()
    {
        // Arrange
        var users = await CreateDefaultUsers();
        var dialog = DialogDbModelFactory.Create(
            Guid.NewGuid(),
            true,
            users[0].ID,
            users[1].ID);
        await CreateSingleDialogFrom(dialog);

        // Act
        await m_dialogRepository.UnBlockDialog(DialogConverter.DbToCoreModel(dialog));

        // Assert
        var models = (from a in m_dbFixture.CreateContext().Dialogs select a).ToList();
        Assert.False(models[0].IsBlockedFlag);
    }

    [Fact]
    public async Task UnBlockDialog_NotFound()
    {
        // Arrange
        var users = await CreateDefaultUsers();
        var dialog = DialogDbModelFactory.Create(
            Guid.NewGuid(),
            true,
            users[0].ID,
            users[1].ID);

        // Act
        await m_dialogRepository.UnBlockDialog(DialogConverter.DbToCoreModel(dialog));

        // Assert
        Assert.True(dialog.IsBlockedFlag);
    }

    [Fact]
    public async Task GetDialogByID_Ok()
    {
        // Arrange
        var users = await CreateDefaultUsers();
        var dialog = DialogDbModelFactory.Create(
            Guid.NewGuid(),
            true,
            users[0].ID,
            users[1].ID);
        await CreateSingleDialogFrom(dialog);

        // Act
        var found = await m_dialogRepository.GetDialogByID(dialog.ID);

        // Assert
        Assert.Equivalent(DialogConverter.DbToCoreModel(dialog), found);
    }

    [Fact]
    public async Task GetDialogByID_NotFound()
    {
        // Arrange

        // Act
        var found = await m_dialogRepository.GetDialogByID(Guid.NewGuid());

        // Assert
        Assert.Null(found);
    }

    // // [Fact]
    // // public async Task ListAllDialogs_Ok()
    // // {
    // //     // Arrange
    // //     List<DialogDbModel> models = [];
    // //     for (byte i = 0; i < 10; ++i)
    // //     {
    // //         models.Add(DialogDbModelFactory.Create(
    // //             new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, i]),
    // //             true,
    // //             new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]),
    // //             new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1])));
    // //     }

    // //     // m_mockDbContextFactory
    // //     //     .MockDialogs
    // //     //         .Setup(s => s.Select(It.IsAny<Func<DialogDbModel, int>>()))
    // //     //         .Returns(() =>
    // //     //         {
    // //     //             return models.Select(u => DialogConverter.DbToCoreModel(u));
    // //     //         });

    // //     // Act
    // //     var found = await m_dialogRepository.ListAllDialogs();

    // //     // Assert
    // //     Assert.Null(found);
    // // }

    [Fact]
    public async Task Update_Ok()
    {
        // Arrange
        var users = await CreateDefaultUsers();
        var dialog = DialogDbModelFactory.Create(
            Guid.NewGuid(),
            false,
            users[0].ID,
            users[1].ID);
        await CreateSingleDialogFrom(dialog);
        var dial_cpy = DialogDbModelFactory.Copy(dialog);
        dial_cpy.IsBlockedFlag = true;

        // Act
        await m_dialogRepository.Update(DialogConverter.DbToCoreModel(dial_cpy));

        // Assert
        var models = (from a in m_dbFixture.CreateContext().Dialogs select a).ToList();
        Assert.True(models[0].IsBlockedFlag);
    }

    [Fact]
    public async Task Update_NotFound()
    {
        // Arrange
        var users = await CreateDefaultUsers();
        var dialog = DialogDbModelFactory.Create(
            Guid.NewGuid(),
            false,
            users[0].ID,
            users[1].ID);
        var dial_cpy = DialogDbModelFactory.Copy(dialog);

        // Act
        await m_dialogRepository.Update(DialogConverter.DbToCoreModel(dial_cpy));

        // Assert
        Assert.Equivalent(dialog, dial_cpy);
    }
}
