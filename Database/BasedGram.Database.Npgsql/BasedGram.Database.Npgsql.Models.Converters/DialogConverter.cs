using System.Diagnostics.CodeAnalysis;
using BasedGram.Common.Core;

using BasedGram.Database.Npgsql.Models;

namespace BasedGram.Database.Npgsql.Models.Converters;

public static class DialogConverter
{
    [return: NotNullIfNotNull(nameof(model))]
    public static Dialog? DbToCoreModel(DialogDbModel? model)
    {
        return model is not null
        ? new(model.ID, model.IsBlockedFlag, model.CreatorID, model.ColocutorID)
        : null;
    }

    [return: NotNullIfNotNull(nameof(model))]
    public static DialogDbModel? CoreToDbModel(Dialog? model)
    {
        return model is not null
        ? new(model.ID, model.IsBlockedFlag, model.CreatorID, model.TargetID)
        : null;
    }
}