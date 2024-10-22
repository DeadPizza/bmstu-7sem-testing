using BasedGram.Common.Core;
using BasedGram.Database.Core.Repositories;
using BasedGram.Services.DialogService;
using BasedGram.Services.P2PService;
using BasedGram.Services.DialogService.Exceptions;

using Serilog;
using BasedGram.Services.P2PService.DTO;

namespace BasedGram.Services.DialogService;

public class DialogService : IDialogService
{
    private readonly IDialogRepository m_DialogRepository;
    private readonly IMessageRepository m_MessageRepository;
    private readonly IP2PService m_p2pservice;
    private readonly ILogger m_logger = Log.ForContext<DialogService>();
    public DialogService(IDialogRepository dialogRepository, IMessageRepository messageRepository, IP2PService p2pservice)
    {
        m_DialogRepository = dialogRepository;
        m_MessageRepository = messageRepository;
        m_p2pservice = p2pservice;
    }

    public void SetCallbacks(IP2PService service)
    {
        service ??= m_p2pservice;

        m_logger.Verbose("SetCallbacks() enter");
        service.OnSyncData(async () =>
        {
            m_logger.Verbose("OnSyncData() enter");
            // Console.WriteLine($"sending dialogs: begin");
            var dials = await m_DialogRepository.ListAllDialogs();
            // Console.WriteLine($"sending dialogs: {dials.Count}");
            m_logger.Information($"Sending {dials.Count} dialogs");
            await service.SendDialogList(dials);
            var msgs = await m_MessageRepository.GetAllMessages();
            m_logger.Information($"Sending {msgs.Count} messages");
            // Console.WriteLine($"sending messages: {msgs.Count}");
            foreach (var message in msgs)
            {
                await service.SendMessage(message);
            }

            var files = Directory.EnumerateFiles("./imgs/").ToList();
            foreach (var file in files)
            {
                var fname = file.Split('.').Last();
                Console.WriteLine(fname);
                // await service.SendImage(new Image())
            }

            m_logger.Verbose("OnSyncData() exit");
        });

        service.OnDialogListReceive(async (List<Dialog> dialogs) =>
        {
            m_logger.Verbose("OnDialogListReceive() enter");
            m_logger.Information($"Receiving {dialogs.Count} dialogs");
            foreach (var dial_recv in dialogs)
            {
                await m_DialogRepository.Update(dial_recv);
            }
            m_logger.Verbose("OnDialogListReceive() exit");
        });

        service.OnMessageReceive(async (Message message) =>
        {
            m_logger.Verbose("OnMessageReceive() enter");
            m_MessageRepository.UpdateMessage(message);
            m_logger.Verbose("OnMessageReceive() exit");
        });

        service.OnImageReceive(async (P2PService.DTO.Image img) =>
        {
            await SaveImage(img);
        });

        m_logger.Verbose("SetCallbacks() exit");
    }
    public async Task<Dialog> CreateDialog(User creator, User target)
    {
        m_logger.Verbose("CreateDialog() enter");
        try
        {
            var all_dialogs = await m_DialogRepository.ListAllDialogs();

            if (all_dialogs.Any(dial => (
                (dial.CreatorID == creator.ID && dial.TargetID == target.ID)
                || (dial.CreatorID == target.ID && dial.TargetID == creator.ID))))
            {
                m_logger.Error("Dialog {@Dialog} already exists!", new { CreatorID = creator.ID, TargetID = target.ID });
                throw new CreateDialogExistsException("Dialog already exists");
            }

            var dialog = new Dialog(Guid.NewGuid(), false, creator.ID, target.ID);
            await m_DialogRepository.CreateDialog(dialog);
            if (m_p2pservice is not null)
            {
                await m_p2pservice.SendDialogList([dialog]);
            }

            m_logger.Information("Dialog {@Dialog} created", new { CreatorID = creator.ID, TargetID = target.ID });
            m_logger.Verbose("CreateDialog() exit");
            return dialog;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public async Task DeleteDialog(Dialog dialog)
    {
        m_logger.Verbose("CreateDialog() enter");
        try
        {
            await m_DialogRepository.DeleteDialog(dialog);
            m_logger.Information("Dialog {@Dialog} deleted", new { dialog.CreatorID, dialog.TargetID });
            m_logger.Verbose("CreateDialog() exit");
        }
        catch (System.Exception)
        {
            throw new DeleteDialogNotExistsException();
        }
    }

    public async Task BlockDialog(Dialog dialog)
    {
        m_logger.Verbose("BlockDialog() enter");
        await m_DialogRepository.BlockDialog(dialog);
        m_logger.Information("Dialog {@Dialog} blocked", new { dialog.CreatorID, dialog.TargetID });
        m_logger.Verbose("BlockDialog() exit");
    }

    public Task<Dialog> GetDialog(Guid guid)
    {
        m_logger.Verbose("GetDialog() enter");
        try
        {
            var dialog = m_DialogRepository.GetDialogByID(guid) ?? throw new Exception();
            m_logger.Verbose("GetDialog() exit");
            return dialog!;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<List<Dialog>> GetUserDialogs(User user)
    {
        m_logger.Verbose("GetUserDialogs() enter");
        try
        {
            var all_dials = await m_DialogRepository.ListAllDialogs();

            all_dials.RemoveAll(dial => !(dial.CreatorID == user.ID || dial.TargetID == user.ID));

            m_logger.Verbose("GetUserDialogs() exit");
            return all_dials;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public async Task<List<Message>> GetDialogMessages(User user, Dialog dialog)
    {
        m_logger.Verbose("GetDialogMessages() enter");
        var tmp = await m_MessageRepository.GetUserMessagesInDialog(user, dialog);
        tmp.Sort((x, y) => DateTime.Compare(x.SendTime, y.SendTime));
        m_logger.Verbose("GetDialogMessages() exit");
        return tmp;
    }

    public async Task<List<Message>> GetAllMessages(Dialog dialog)
    {
        m_logger.Verbose("GetAllMessages() enter");
        var tmp = await m_MessageRepository.GetAllMessagesInDialog(dialog);
        tmp.Sort((x, y) => DateTime.Compare(x.SendTime, y.SendTime));
        m_logger.Verbose("GetAllMessages() exit");
        return tmp;
    }

    public async Task<Message> CreateMessage(User user, Dialog dialog, string msg, string embedding)
    {
        m_logger.Verbose("CreateMessage() enter");
        Message _msg = new(Guid.NewGuid(), user.ID, dialog.ID, DateTime.Now, msg, false, Reaction.NoReaction, embedding);
        if(m_p2pservice is not null)
        {
            await m_p2pservice.SendMessage(_msg);
        }
        await m_MessageRepository.CreateMessage(_msg);
        m_logger.Information("Message {@Message} created", new { _msg.ID, _msg.DialogID });
        m_logger.Verbose("CreateMessage() exit");
        return _msg;
    }

    public async Task UpdateMessage(Message msg)
    {
        m_logger.Verbose("UpdateMessage() enter");
        await m_MessageRepository.UpdateMessage(msg);
        m_logger.Information("Message {@Message} updated", new { msg.ID, msg.DialogID });
        m_logger.Verbose("UpdateMessage() exit");
    }

    public async Task UnblockDialog(Dialog dialog)
    {
        m_logger.Verbose("BlockDialog() enter");
        await m_DialogRepository.UnBlockDialog(dialog);
        m_logger.Information("Dialog {@Dialog} blocked", new { dialog.CreatorID, dialog.TargetID });
        m_logger.Verbose("BlockDialog() exit");
    }

    public async Task SaveImage(Image image)
    {
        var imagePath = $"./imgs/{image.Name}";
        // Set the path where the image will be saved
        using (var stream = new FileStream(imagePath, FileMode.Create))
        {
            image._Image.CopyTo(stream);
        }
    }
}
