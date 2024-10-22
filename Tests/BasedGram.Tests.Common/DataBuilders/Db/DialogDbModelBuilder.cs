using BasedGram.Database.Npgsql.Models;

namespace BasedGram.Tests.Common.DataBuilders.Db;

public class DialogDbModelBuilder
{
    private DialogDbModel _dialogDbModel = new();

    public DialogDbModelBuilder WithID(Guid id)
    {
        _dialogDbModel.ID = id;
        return this;
    }

    public DialogDbModelBuilder WithIsBlockedFlag(bool isBlockedFlag)
    {
        _dialogDbModel.IsBlockedFlag = isBlockedFlag;
        return this;
    }

    public DialogDbModelBuilder WithCreatorID(Guid creatorID)
    {
        _dialogDbModel.CreatorID = creatorID;
        return this;
    }

    public DialogDbModelBuilder WithColocutorID(Guid colocutorID)
    {
        _dialogDbModel.ColocutorID = colocutorID;
        return this;
    }

    public DialogDbModel Build()
    {
        var dialogDbModel = _dialogDbModel;
        _dialogDbModel = new DialogDbModel(); // Сбрасываем текущее состояние после вызова Build
        return dialogDbModel;
    }

    // Метод Copy для возможности клонирования с изменениями
    public DialogDbModelBuilder Copy(DialogDbModel dialogDbModel)
    {
        _dialogDbModel = new DialogDbModel(dialogDbModel.ID, dialogDbModel.IsBlockedFlag,
                                            dialogDbModel.CreatorID, dialogDbModel.ColocutorID);
        return this;
    }
}
