using BasedGram.Database.Npgsql.Models.Converters;
using BasedGram.Database.Npgsql.Models;
using BasedGram.Database.NpgsqlRepositories;
using BasedGram.Tests.Common.Factories.Core;
using BasedGram.Tests.Common.Factories.Db;
using Moq;
using BasedGram.Common.Core;

namespace BasedGram.Tests.Unit.Repositories;

public class TestDialogRepository : TestRepositoryBase
{
    protected readonly DialogRepository m_dialogRepository;
    public TestDialogRepository() : base()
    {
        m_dialogRepository = new(m_mockDbContextFactory.MockContext.Object);
    }

    [Fact]
    public async Task CreateDialog_Ok()
    {
        // Arrange
        List<DialogDbModel> models = [];
        var dialog = DialogDbModelFactory.Create(
            new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]),
            false,
            new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]),
            new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1]));

        m_mockDbContextFactory
            .MockDialogs.Setup(s =>
                s.AddAsync(It.IsAny<DialogDbModel>(), default)
            )
            .Callback<DialogDbModel, CancellationToken>(
                (a, token) => models.Add(a)
            );

        // Act
        await m_dialogRepository.CreateDialog(DialogConverter.DbToCoreModel(dialog));

        // Assert
        Assert.Single(models);
        Assert.Equivalent(dialog, models[0]);
    }

    [Fact]
    public async Task CreateDialog_NonUniqueException()
    {
        // Arrange
        var dialog = DialogDbModelFactory.Create(
            new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]),
            false,
            new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]),
            new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1]));
        List<DialogDbModel> models = [dialog];

        m_mockDbContextFactory
            .MockDialogs.Setup(s =>
                s.AddAsync(It.IsAny<DialogDbModel>(), default)
            )
            .Callback<DialogDbModel, CancellationToken>(
                (a, token) => throw new Exception()
            );

        // Act
        async Task action() => await m_dialogRepository.CreateDialog(DialogConverter.DbToCoreModel(dialog));

        // Assert
        await Assert.ThrowsAsync<Exception>(action);
    }

    [Fact]
    public async Task DeleteDialog_Ok()
    {
        // Arrange
        var dialog = DialogDbModelFactory.Create(
            new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]),
            false,
            new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]),
            new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1]));
        List<DialogDbModel> models = [dialog];

        m_mockDbContextFactory
            .MockDialogs.Setup(s =>
                s.Remove(It.IsAny<DialogDbModel>())
            )
            .Callback<DialogDbModel>(
                (a) => models.RemoveAll(x => x.ID == a.ID)
            );

        m_mockDbContextFactory
            .MockDialogs
                .Setup(s => s.FindAsync(It.IsAny<Guid>()))
                .ReturnsAsync(dialog);

        // Act
        await m_dialogRepository.DeleteDialog(DialogConverter.DbToCoreModel(dialog));

        // Assert
        Assert.Empty(models);
    }

    [Fact]
    public async Task DeleteDialog_NotFound()
    {
        // Arrange
        var dialog = DialogDbModelFactory.Create(
            new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]),
            false,
            new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]),
            new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1]));

        var dialog2 = DialogDbModelFactory.Create(
            new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 228]),
            false,
            new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]),
            new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1]));

        List<DialogDbModel> models = [dialog];

        m_mockDbContextFactory
            .MockDialogs.Setup(s =>
                s.Remove(It.IsAny<DialogDbModel>())
            )
            .Callback<DialogDbModel>(
                (a) => models.RemoveAll(x => x.ID == a.ID)
            );

        m_mockDbContextFactory
            .MockDialogs
                .Setup(s => s.FindAsync(It.IsAny<Guid>()))
                .ReturnsAsync(dialog);

        // Act
        await m_dialogRepository.DeleteDialog(DialogConverter.DbToCoreModel(dialog2));

        // Assert
        Assert.Equivalent(models, new List<DialogDbModel>([dialog]));
    }

    [Fact]
    public async Task BlockDialog_Ok()
    {
        // Arrange
        var dialog = DialogDbModelFactory.Create(
            new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]),
            false,
            new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]),
            new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1]));
        List<DialogDbModel> models = [dialog];

        m_mockDbContextFactory
            .MockDialogs.Setup(s =>
                s.Update(It.IsAny<DialogDbModel>())
            )
            .Callback<DialogDbModel>(
                (a) => models[0] = dialog
            );

        m_mockDbContextFactory
            .MockDialogs
                .Setup(s => s.FindAsync(It.IsAny<Guid>()))
                .ReturnsAsync(dialog);

        // Act
        await m_dialogRepository.BlockDialog(DialogConverter.DbToCoreModel(dialog));

        // Assert
        Assert.True(dialog.IsBlockedFlag);
    }

    [Fact]
    public async Task BlockDialog_NotFound()
    {
        // Arrange
        var dialog = DialogDbModelFactory.Create(
            new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]),
            false,
            new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]),
            new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1]));
        List<DialogDbModel> models = [dialog];

        m_mockDbContextFactory
            .MockDialogs.Setup(s =>
                s.Update(It.IsAny<DialogDbModel>())
            )
            .Callback<DialogDbModel>(
                (a) => models[0] = dialog
            );

        // Act
        await m_dialogRepository.BlockDialog(DialogConverter.DbToCoreModel(dialog));

        // Assert
        Assert.False(dialog.IsBlockedFlag);
    }

    [Fact]
    public async Task UnBlockDialog_Ok()
    {
        // Arrange
        var dialog = DialogDbModelFactory.Create(
            new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]),
            true,
            new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]),
            new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1]));
        List<DialogDbModel> models = [dialog];

        m_mockDbContextFactory
            .MockDialogs.Setup(s =>
                s.Update(It.IsAny<DialogDbModel>())
            )
            .Callback<DialogDbModel>(
                (a) => models[0] = dialog
            );

        m_mockDbContextFactory
            .MockDialogs
                .Setup(s => s.FindAsync(It.IsAny<Guid>()))
                .ReturnsAsync(dialog);

        // Act
        await m_dialogRepository.UnBlockDialog(DialogConverter.DbToCoreModel(dialog));

        // Assert
        Assert.False(dialog.IsBlockedFlag);
    }

    [Fact]
    public async Task UnBlockDialog_NotFound()
    {
        // Arrange
        var dialog = DialogDbModelFactory.Create(
            new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]),
            true,
            new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]),
            new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1]));
        List<DialogDbModel> models = [dialog];

        m_mockDbContextFactory
            .MockDialogs.Setup(s =>
                s.Update(It.IsAny<DialogDbModel>())
            )
            .Callback<DialogDbModel>(
                (a) => models[0] = dialog
            );

        // Act
        await m_dialogRepository.UnBlockDialog(DialogConverter.DbToCoreModel(dialog));

        // Assert
        Assert.True(dialog.IsBlockedFlag);
    }

    [Fact]
    public async Task GetDialogByID_Ok()
    {
        // Arrange
        var dialog = DialogDbModelFactory.Create(
            new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]),
            true,
            new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]),
            new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1]));
        List<DialogDbModel> models = [dialog];

        m_mockDbContextFactory
            .MockDialogs
                .Setup(s => s.FindAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Object[] to_find) =>
                {
                    return models.FirstOrDefault(u => u.ID == (Guid)(to_find[0]));
                });

        // Act
        var found = await m_dialogRepository.GetDialogByID(dialog.ID);

        // Assert
        Assert.Equivalent(DialogConverter.DbToCoreModel(dialog), found);
    }

    [Fact]
    public async Task GetDialogByID_NotFound()
    {
        // Arrange
        var dialog = DialogDbModelFactory.Create(
            new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]),
            true,
            new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]),
            new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1]));
        var dialog2 = DialogDbModelFactory.Create(
            new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 12]),
            true,
            new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]),
            new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1]));
        List<DialogDbModel> models = [dialog];

        m_mockDbContextFactory
            .MockDialogs
                .Setup(s => s.FindAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Object[] to_find) =>
                {
                    return models.FirstOrDefault(u => u.ID == (Guid)(to_find[0]));
                });

        // Act
        var found = await m_dialogRepository.GetDialogByID(dialog2.ID);

        // Assert
        Assert.Null(found);
    }

    // [Fact]
    // public async Task ListAllDialogs_Ok()
    // {
    //     // Arrange
    //     List<DialogDbModel> models = [];
    //     for (byte i = 0; i < 10; ++i)
    //     {
    //         models.Add(DialogDbModelFactory.Create(
    //             new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, i]),
    //             true,
    //             new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]),
    //             new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1])));
    //     }

    //     // m_mockDbContextFactory
    //     //     .MockDialogs
    //     //         .Setup(s => s.Select(It.IsAny<Func<DialogDbModel, int>>()))
    //     //         .Returns(() =>
    //     //         {
    //     //             return models.Select(u => DialogConverter.DbToCoreModel(u));
    //     //         });

    //     // Act
    //     var found = await m_dialogRepository.ListAllDialogs();

    //     // Assert
    //     Assert.Null(found);
    // }

    [Fact]
    public async Task Update_Ok()
    {
        // Arrange
        var dialog = DialogDbModelFactory.Create(
           new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]),
           true,
           new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]),
           new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1]));
        List<DialogDbModel> models = [dialog];

        m_mockDbContextFactory
            .MockDialogs
                .Setup(s => s.FindAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Object[] to_find) =>
                {
                    return models.FirstOrDefault(u => u.ID == (Guid)(to_find[0]));
                });

        // Act
        var dial_cpy = DialogDbModelFactory.Copy(dialog);
        dial_cpy.IsBlockedFlag = true;
        await m_dialogRepository.Update(DialogConverter.DbToCoreModel(dial_cpy));

        // Assert
        Assert.Equivalent(dialog, dial_cpy);
    }

    [Fact]
    public async Task Update_NotFound()
    {
        // Arrange
        var dialog = DialogDbModelFactory.Create(
           new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]),
           true,
           new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]),
           new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1]));
        List<DialogDbModel> models = [dialog];
        var dialog_non_exist = DialogDbModelFactory.Create(
           new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 25]),
           true,
           new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]),
           new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1]));

        m_mockDbContextFactory
            .MockDialogs
                .Setup(s => s.FindAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Object[] to_find) =>
                {
                    return models.FirstOrDefault(u => u.ID == (Guid)(to_find[0]));
                });

        // Act
        var dial_cpy = DialogDbModelFactory.Copy(dialog);
        await m_dialogRepository.Update(DialogConverter.DbToCoreModel(dialog_non_exist));

        // Assert
        Assert.Equivalent(dialog, dial_cpy);
    }
}
