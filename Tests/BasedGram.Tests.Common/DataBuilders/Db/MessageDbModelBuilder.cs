using BasedGram.Database.Npgsql.Models;

namespace BasedGram.Tests.Common.DataBuilders.Db;

public class MessageDbModelBuilder
{
    private MessageDbModel _messageDbModel = new();

    public MessageDbModelBuilder WithID(Guid id)
    {
        _messageDbModel.ID = id;
        return this;
    }

    public MessageDbModelBuilder WithSenderID(Guid senderID)
    {
        _messageDbModel.SenderID = senderID;
        return this;
    }

    public MessageDbModelBuilder WithDialogID(Guid dialogID)
    {
        _messageDbModel.DialogID = dialogID;
        return this;
    }

    public MessageDbModelBuilder WithSentTime(DateTime sentTime)
    {
        _messageDbModel.SentTime = sentTime;
        return this;
    }

    public MessageDbModelBuilder WithContent(string? content)
    {
        _messageDbModel.Content = content;
        return this;
    }

    public MessageDbModelBuilder WithIsReadFlag(bool isReadFlag)
    {
        _messageDbModel.isReadFlag = isReadFlag;
        return this;
    }

    public MessageDbModelBuilder WithReactionState(int reactionState)
    {
        _messageDbModel.ReactionState = reactionState;
        return this;
    }

    public MessageDbModelBuilder WithEmbedding(string? embedding)
    {
        _messageDbModel.Embedding = embedding;
        return this;
    }

    public MessageDbModel Build()
    {
        var messageDbModel = _messageDbModel;
        _messageDbModel = new MessageDbModel(); // Сбрасываем текущее состояние после вызова Build
        return messageDbModel;
    }

    // Метод Copy для возможности клонирования с изменениями
    public MessageDbModelBuilder Copy(MessageDbModel messageDbModel)
    {
        _messageDbModel = new MessageDbModel(
            messageDbModel.ID,
            messageDbModel.SenderID,
            messageDbModel.DialogID,
            messageDbModel.SentTime,
            messageDbModel.Content,
            messageDbModel.isReadFlag,
            messageDbModel.ReactionState,
            messageDbModel.Embedding
        );
        return this;
    }
}
