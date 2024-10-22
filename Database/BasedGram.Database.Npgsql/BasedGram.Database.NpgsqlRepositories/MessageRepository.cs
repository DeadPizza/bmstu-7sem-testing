using BasedGram.Common.Core;
using BasedGram.Database.Context;
using BasedGram.Database.Core.Repositories;
using BasedGram.Database.Npgsql.Models.Converters;
using Microsoft.EntityFrameworkCore;

using Serilog;

namespace BasedGram.Database.NpgsqlRepositories;

public class MessageRepository(BasedGramNpgsqlDbContext context) : IMessageRepository
{
    private readonly BasedGramNpgsqlDbContext m_context = context;
    private readonly ILogger m_logger = Log.ForContext<MessageRepository>();
    public async Task CreateMessage(Message message)
    {
        m_logger.Verbose("CreateMessage() enter");
        await m_context.Messages.AddAsync(MessageConverter.CoreToDbModel(message));
        await m_context.SaveChangesAsync();
        m_logger.Verbose("CreateMessage() exit");
    }

    public async Task DeleteMessage(Message message)
    {
        m_logger.Verbose("DeleteMessage() enter");
        var foundDbModel = await m_context.Messages.FindAsync(message.ID);
        // Console.WriteLine("NIGGER : {0}", foundDbModel is not null);
        if (foundDbModel is not null)
        {
            m_context.Messages.Remove(foundDbModel!);
            await m_context.SaveChangesAsync();
        }
        m_logger.Verbose("DeleteMessage() exit");
    }

    public async Task<Message?> GetMessageByID(Guid guid)
    {
        m_logger.Verbose("GetMessageByID() enter");
        var messageDbModel = await m_context.Messages.FindAsync(guid);
        m_logger.Verbose("GetMessageByID() exit");
        return MessageConverter.DbToCoreModel(messageDbModel);
    }

    public Task<List<Message>> GetUserMessagesInDialog(User user, Dialog dialog)
    {
        m_logger.Verbose("GetUserMessagesInDialog()");
        return m_context.Messages
            .Where(m => m.SenderID == user.ID && m.DialogID == dialog.ID)
            .Select(m => MessageConverter.DbToCoreModel(m))
            .ToListAsync();
    }

    public async Task UpdateMessage(Message message)
    {
        m_logger.Verbose("UpdateMessage() enter");
        var messageDbModel = await m_context.Messages.FindAsync(message.ID);
        if (messageDbModel is not null)
        {
            var converted = MessageConverter.CoreToDbModel(message);
            messageDbModel.isReadFlag = converted.isReadFlag;
            messageDbModel.ReactionState = converted.ReactionState;
            messageDbModel.Content = converted.Content;
            // Console.Write(m_context.Entry(messageDbModel).State);
            // m_context.Messages.Update(messageDbModel);
            // messageDbModel = MessageConverter.CoreToDbModel(message);
            // m_context.ChangeTracker.DetectChanges();
            // m_context.Messages.Update(messageDbModel);
        }
        else
        {
            await m_context.Messages.AddAsync(MessageConverter.CoreToDbModel(message));
        }
        await m_context.SaveChangesAsync();
        m_logger.Verbose("UpdateMessage() exit");
    }

    public Task<List<Message>> GetAllMessagesInDialog(Dialog dialog)
    {
        m_logger.Verbose("GetAllMessagesInDialog()");
        return m_context.Messages
            .Where(m => m.DialogID == dialog.ID)
            .Select(m => MessageConverter.DbToCoreModel(m))
            .ToListAsync();
    }

    public Task<List<Message>> GetAllMessages()
    {
        m_logger.Verbose("GetAllMessages()");
        return m_context.Messages
            .Select(m => MessageConverter.DbToCoreModel(m))
            .ToListAsync();
    }
}