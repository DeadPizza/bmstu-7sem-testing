using BasedGram.Database.Npgsql.Models;

namespace BasedGram.Tests.Common.Factories.Db;

public class MessageDbModelFactory
{
    public static MessageDbModel Create(
        Guid id,
        Guid senderId,
        Guid dialogId,
        DateTime sentTime,
        string? content,
        bool isReadFlag,
        int reactionState,
        string embedding)
    {
        return new MessageDbModel(id, senderId, dialogId, sentTime, content, isReadFlag, reactionState, embedding);
    }

    public static MessageDbModel Copy(MessageDbModel other)
    {
        return new MessageDbModel(
            other.ID,
            other.SenderID,
            other.DialogID,
            other.SentTime,
            other.Content,
            other.isReadFlag,
            other.ReactionState,
            other.Embedding
        );
    }

    public static MessageDbModel CreateEmpty()
    {
        return new MessageDbModel();
    }
}