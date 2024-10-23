using BasedGram.Database.Core.Repositories;
using BasedGram.Database.Context;
using BasedGram.Common.Core;
using BasedGram.Database.Npgsql.Models.Converters;
using Microsoft.EntityFrameworkCore;

using Serilog;

namespace BasedGram.Database.NpgsqlRepositories;

public class DialogRepository(BasedGramNpgsqlDbContext context) : IDialogRepository
{
    private readonly BasedGramNpgsqlDbContext m_context = context;
    private readonly ILogger m_logger = Log.ForContext<DialogRepository>();
    public async Task CreateDialog(Dialog dialog)
    {
        m_logger.Verbose("CreateDialog() enter");
        await m_context.Dialogs.AddAsync(DialogConverter.CoreToDbModel(dialog));
        await m_context.SaveChangesAsync();
        m_logger.Verbose("CreateDialog() exit");
    }

    public async Task DeleteDialog(Dialog dialog)
    {
        m_logger.Verbose("DeleteDialog() enter");
        var foundDbModel = await m_context.Dialogs.FindAsync(dialog.ID!);
        if (foundDbModel is not null)
        {
            m_context.Dialogs.Remove(foundDbModel!);
            await m_context.SaveChangesAsync();
        }
        m_logger.Verbose("DeleteDialog() exit");
    }

    public async Task BlockDialog(Dialog dialog)
    {
        m_logger.Verbose("BlockDialog() enter");
        var dialogDbModel = await m_context.Dialogs.FindAsync(dialog.ID);
        if (dialogDbModel is not null)
        {
            dialogDbModel.IsBlockedFlag = true;
            m_context.Dialogs.Update(dialogDbModel);
            await m_context.SaveChangesAsync();
        }
        m_logger.Verbose("BlockDialog() exit");
    }
    public async Task UnBlockDialog(Dialog dialog)
    {
        m_logger.Verbose("BlockDialog() enter");
        var dialogDbModel = await m_context.Dialogs.FindAsync(dialog.ID);
        if (dialogDbModel is not null)
        {
            dialogDbModel.IsBlockedFlag = false;
            m_context.Dialogs.Update(dialogDbModel);
            await m_context.SaveChangesAsync();
        }
        m_logger.Verbose("BlockDialog() exit");
    }
    public async Task<Dialog?> GetDialogByID(Guid guid)
    {
        m_logger.Verbose("GetDialogByID() enter");
        var dialogDbModel = await m_context.Dialogs.FindAsync(guid);
        m_logger.Verbose("GetDialogByID() exit");
        return DialogConverter.DbToCoreModel(dialogDbModel);
    }
    
    public Task<List<Dialog>> ListAllDialogs()
    {
        m_logger.Verbose("ListAllDialogs()");
        return m_context.Dialogs
            .Select(d => DialogConverter.DbToCoreModel(d))
            .ToListAsync();
    }
    public async Task Update(Dialog dialog)
    {
        m_logger.Verbose("Update() enter");
        var dialogDbModel = await m_context.Dialogs.FindAsync(dialog.ID);
        if (dialogDbModel is not null)
        {
            dialogDbModel.IsBlockedFlag = dialog.IsBlockedFlag;
            // m_context.Dialogs.Update(DialogConverter.CoreToDbModel(dialog));
        }
        await m_context.SaveChangesAsync();
        m_logger.Verbose("Update() exit");
    }

}
