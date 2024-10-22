using BasedGram.Common.Core;

namespace BasedGram.Tests.Common.Factories.Core;

public class MessageFactory
{
    public static Message Create(
        Guid id,
        Guid senderId,
        Guid dialogId,
        DateTime sendTime,
        string? content,
        bool isRead,
        Reaction reactionState,
        string embedding)
    {
        return new Message(id, senderId, dialogId, sendTime, content, isRead, reactionState, embedding);
    }

    public static Message Copy(Message other)
    {
        return new Message(
            other.ID,
            other.SenderID,
            other.DialogID,
            other.SendTime,
            other.Content,
            other.isReadFlag,
            other.ReactionState,
            other.Embedding
        );
    }

    public static Message CreateEmpty()
    {
        return new Message();
    }
}