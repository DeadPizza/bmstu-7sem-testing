using BasedGram.Database.Npgsql.Models;

namespace BasedGram.Tests.Common.Factories.Db;

public class DialogDbModelFactory
{
    public static DialogDbModel Create(
        Guid id,
        bool isBlockedFlag,
        Guid creatorId,
        Guid colocutorId)
    {
        return new DialogDbModel(id, isBlockedFlag, creatorId, colocutorId);
    }

    public static DialogDbModel Copy(DialogDbModel other)
    {
        return new DialogDbModel(
            other.ID,
            other.IsBlockedFlag,
            other.CreatorID,
            other.ColocutorID
        );
    }

    public static DialogDbModel CreateEmpty()
    {
        return new DialogDbModel();
    }
}