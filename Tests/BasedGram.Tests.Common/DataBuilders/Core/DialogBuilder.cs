using BasedGram.Common.Core;

namespace BasedGram.Tests.Common.DataBuilders.Core;

public class DialogBuilder
{
    private Dialog _dialog = new();

    public DialogBuilder WithID(Guid id)
    {
        _dialog.ID = id;
        return this;
    }

    public DialogBuilder WithIsBlockedFlag(bool isBlockedFlag)
    {
        _dialog.IsBlockedFlag = isBlockedFlag;
        return this;
    }

    public DialogBuilder WithCreatorID(Guid creatorID)
    {
        _dialog.CreatorID = creatorID;
        return this;
    }

    public DialogBuilder WithTargetID(Guid targetID)
    {
        _dialog.TargetID = targetID;
        return this;
    }

    public Dialog Build()
    {
        var dialog = _dialog;
        _dialog = new Dialog(); // Сбрасываем текущее состояние после вызова Build
        return dialog;
    }

    // Метод Copy для возможности клонирования с изменениями
    public DialogBuilder Copy(Dialog dialog)
    {
        _dialog = new Dialog(dialog.ID, dialog.IsBlockedFlag, dialog.CreatorID, dialog.TargetID);
        return this;
    }
}
