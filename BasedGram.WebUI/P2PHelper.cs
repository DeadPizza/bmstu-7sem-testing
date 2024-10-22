using BasedGram.Common.Core;
using BasedGram.Database.Core.Repositories;
using BasedGram.Services.P2PService;

namespace BasedGram.WebUI;

public class P2PHelper
{
    private readonly IUserRepository userRepository;
    private readonly IDialogRepository dialogRepository;
    private readonly IMessageRepository messageRepository;

    public async Task OnSyncData(P2PService p2p_serv)
    {
        Console.WriteLine("Sync data");
        var all_users_list = await userRepository.ListAllUsers();
        all_users_list.AddRange(await userRepository.ListAllAdmins());
        var all_dialogs_list = await dialogRepository.ListAllDialogs();
        var all_msgs_list = await messageRepository.GetAllMessages();

        await p2p_serv.SendUserList(all_users_list);
        await p2p_serv.SendDialogList(all_dialogs_list);
        all_msgs_list.ForEach(async (msg) => { await p2p_serv.SendMessage(msg); });
    }

    public async Task OnUserListReceive(List<User> users)
    {
        users.ForEach(async (user) =>
        {
            await userRepository.UpdateUser(user);
        });
    }

    public async Task OnDialogListReceive(List<Dialog> dialogs)
    {
        dialogs.ForEach(async (dialog) =>
        {
            await dialogRepository.Update(dialog);
        });
    }


    public async Task OnMessageReceive(Message message)
    {
        await messageRepository.UpdateMessage(message);
    }
}
