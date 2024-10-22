using BasedGram.Common.Core;

namespace BasedGram.Tests.Common.DataBuilders.Core;

public class MessageBuilder
{
    private Message _message = new();

    public MessageBuilder WithID(Guid id)
    {
        _message.ID = id;
        return this;
    }

    public MessageBuilder WithSenderID(Guid senderID)
    {
        _message.SenderID = senderID;
        return this;
    }

    public MessageBuilder WithDialogID(Guid dialogID)
    {
        _message.DialogID = dialogID;
        return this;
    }

    public MessageBuilder WithSendTime(DateTime sendTime)
    {
        _message.SendTime = sendTime;
        return this;
    }

    public MessageBuilder WithContent(string? content)
    {
        _message.Content = content;
        return this;
    }

    public MessageBuilder WithIsReadFlag(bool isReadFlag)
    {
        _message.isReadFlag = isReadFlag;
        return this;
    }

    public MessageBuilder WithReactionState(Reaction reactionState)
    {
        _message.ReactionState = reactionState;
        return this;
    }

    public MessageBuilder WithEmbedding(string? embedding)
    {
        _message.Embedding = embedding;
        return this;
    }

    public Message Build()
    {
        var message = _message;
        _message = new Message(); // Сбрасываем текущее состояние после вызова Build
        return message;
    }

    // Метод Copy для возможности клонирования с изменениями
    public MessageBuilder Copy(Message message)
    {
        _message = new Message(message.ID, message.SenderID, message.DialogID, message.SendTime,
                               message.Content, message.isReadFlag, message.ReactionState, message.Embedding);
        return this;
    }
}
