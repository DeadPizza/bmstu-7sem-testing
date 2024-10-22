using BasedGram.Common.Core;
using BasedGram.Database.Core;

namespace BasedGram.Database.Core.Repositories;

public interface IMessageRepository
{
    Task CreateMessage(Message message);
    Task UpdateMessage(Message message);
    Task DeleteMessage(Message message);
    Task<Message?> GetMessageByID(Guid guid);
    Task<List<Message>> GetUserMessagesInDialog(User user, Dialog dialog);
    Task<List<Message>> GetAllMessagesInDialog(Dialog dialog);
    Task<List<Message>> GetAllMessages();
}