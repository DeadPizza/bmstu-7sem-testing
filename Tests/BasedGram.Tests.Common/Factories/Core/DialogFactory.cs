using BasedGram.Common.Core;

namespace BasedGram.Tests.Common.Factories.Core;

public class DialogFactory
{
    public static Dialog Create(
        Guid id,
        bool isBlockedFlag,
        Guid creatorId,
        Guid targetId
    )
    {
        return new Dialog(id, isBlockedFlag, creatorId, targetId);
    }

    public static Dialog Copy(Dialog other)
    {
        return new Dialog(other.ID, other.IsBlockedFlag, other.CreatorID, other.TargetID);
    }

    public static Dialog CreateEmpty()
    {
        return new Dialog();
    }
}