using BasedGram.Common.Core;

using BasedGram.Services.P2PService;

namespace BasedGram.Services.DialogService;

public interface IDialogService
{
    void SetCallbacks(IP2PService service);
    Task<Dialog> CreateDialog(User creator, User target);
    Task<Dialog> GetDialog(Guid guid);
    Task DeleteDialog(Dialog dialog);
    Task BlockDialog(Dialog dialog);
    Task UnblockDialog(Dialog dialog);
    Task<List<Dialog>> GetUserDialogs(User user);
    Task<List<Message>> GetDialogMessages(User user, Dialog dialog);
    Task<List<Message>> GetAllMessages(Dialog dialog);
    Task<Message> CreateMessage(User user, Dialog dialog, string msg, string embedding);
    Task UpdateMessage(Message msg);
    Task SaveImage(P2PService.DTO.Image image);
}