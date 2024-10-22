using BasedGram.Common.Core;

namespace BasedGram.Database.Core.Repositories;

public interface IDialogRepository
{
    Task CreateDialog(Dialog dialog);
    Task BlockDialog(Dialog dialog);
    Task UnBlockDialog(Dialog dialog);
    Task Update(Dialog dialog);
    Task<Dialog?> GetDialogByID(Guid guid);
    Task<List<Dialog>> ListAllDialogs();
    Task DeleteDialog(Dialog dialog);
}