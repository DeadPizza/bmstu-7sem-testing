using BasedGram.Common.Core;
using BasedGram.Services.P2PService.Callbacks;

namespace BasedGram.Services.P2PService;

public interface IP2PService
{
    void RunNode();
    void OnMessageReceive(OnMessageReceiveCallback callback);
    void OnUserListReceive(OnUserListReceiveCallback callback);
    void OnDialogListReceive(OnDialogListReceiveCallback callback);
    void OnImageReceive(OnImageReceiveCallback callback);
    void OnUserAction(OnUserActionCallback callback);
    void OnSyncData(OnSyncDataCallback callback);
    Task SendMessage(Message message);
    Task SendUserList(List<User> users);
    Task SendDialogList(List<Dialog> dialogs);
    Task SendImage(string name, Stream image);
}