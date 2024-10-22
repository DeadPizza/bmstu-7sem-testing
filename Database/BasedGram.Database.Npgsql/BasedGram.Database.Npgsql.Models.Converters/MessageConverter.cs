using System.Diagnostics.CodeAnalysis;
using BasedGram.Common.Core;
using BasedGram.Database.Npgsql.Models;

namespace BasedGram.Database.Npgsql.Models.Converters;

public static class MessageConverter
{
    [return: NotNullIfNotNull(nameof(model))]
    public static Message? DbToCoreModel(MessageDbModel? model)
    {
        if (model is null)
        {
            return null;
        }

        Reaction reaction = Reaction.NoReaction;
        switch (model.ReactionState)
        {
            case 0: reaction = Reaction.NoReaction; break;
            case 1: reaction = Reaction.SenderReaction; break;
            case 2: reaction = Reaction.TargetReaction; break;
            case 3: reaction = Reaction.AllReaction; break;
        }

        return new(model.ID, model.SenderID, model.DialogID, model.SentTime, model.Content, model.isReadFlag, reaction, model.Embedding!);
    }

    [return: NotNullIfNotNull(nameof(model))]
    public static MessageDbModel? CoreToDbModel(Message? model)
    {
        if (model is null)
        {
            return null;
        }

        int reaction = 0;
        switch (model.ReactionState)
        {
            case Reaction.NoReaction: reaction = 0; break;
            case Reaction.SenderReaction: reaction = 1; break;
            case Reaction.TargetReaction: reaction = 2; break;
            case Reaction.AllReaction: reaction = 3; break;
        }

        return new(
            model.ID,
            model.SenderID,
            model.DialogID,
            model.SendTime.ToUniversalTime(),
            model.Content,
            model.isReadFlag,
            reaction,
            model.Embedding!);
    }
}