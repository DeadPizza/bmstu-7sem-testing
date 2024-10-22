using BasedGram.Common.Core;
using BasedGram.Common.Enums;
using BasedGram.Database.Core.Repositories;
using Moq;

namespace BasedGram.Tests.Unit.Services;

public class TestServiceBase
{
    protected List<User> m_users
    {
        get => m_mockUserRepo.m_users;
        set
        {
            m_mockUserRepo.m_users = value;
        }
    }

    protected List<Dialog> m_dialogs = [];
    protected List<Message> m_messages = [];
    public class InMemoryUserRepository(List<User> users) : IUserRepository
    {
        public List<User> m_users = users;

        public async Task CreateUser(User user)
        {
            m_users.Add(user);
        }

        public async Task DeleteUser(User user)
        {
            m_users.Remove(user);
        }

        public async Task<User?> GetUserByID(Guid guid)
        {
            return m_users.FirstOrDefault(u => u.ID == guid);
        }

        public async Task<List<User>> ListAllAdmins()
        {
            return m_users.FindAll(u => u.Role == Role.Admin);
        }

        public async Task<List<User>> ListAllUsers()
        {
            return m_users.FindAll(u => u.Role == Role.User);
        }

        public async Task UpdateUser(User user)
        {
            var target = m_users.FirstOrDefault(us => us.ID == user.ID);
            if (target is not null)
            {
                target.IsFreezed = user.IsFreezed;
                target.Role = user.Role;
            }
        }
    }
    protected InMemoryUserRepository m_mockUserRepo;
    protected Mock<IDialogRepository> m_mockDialogRepo;
    protected Mock<IMessageRepository> m_mockMessageRepo;


    public TestServiceBase()
    {
        m_mockUserRepo = new([]);
        m_mockDialogRepo = new();
        m_mockMessageRepo = new();

        m_mockDialogRepo
            .Setup(r => r.CreateDialog(It.IsAny<Dialog>()))
            .Callback((Dialog d) => m_dialogs.Add(d));

        m_mockDialogRepo
            .Setup(r => r.BlockDialog(It.IsAny<Dialog>()))
            .Callback((Dialog d) =>
            {
                var dialog = m_dialogs.FirstOrDefault(di => di.ID == d.ID);
                if (dialog != null)
                {
                    dialog.IsBlockedFlag = true;
                }
            });

        m_mockDialogRepo
            .Setup(r => r.UnBlockDialog(It.IsAny<Dialog>()))
            .Callback((Dialog d) =>
            {
                var dialog = m_dialogs.FirstOrDefault(di => di.ID == d.ID);
                if (dialog != null)
                {
                    dialog.IsBlockedFlag = false;
                }
            });

        m_mockDialogRepo
            .Setup(r => r.Update(It.IsAny<Dialog>()))
            .Callback((Dialog d) =>
            {
                var dialog = m_dialogs.FirstOrDefault(di => di.ID == d.ID);
                if (dialog != null)
                {
                    dialog.IsBlockedFlag = d.IsBlockedFlag;
                }
            });

        m_mockDialogRepo
            .Setup(r => r.GetDialogByID(It.IsAny<Guid>()))
            .ReturnsAsync((Guid guid) => m_dialogs.First(d => d.ID == guid));

        m_mockDialogRepo
            .Setup(r => r.ListAllDialogs())
            .ReturnsAsync(() => m_dialogs.ToList());

        m_mockDialogRepo
            .Setup(r => r.DeleteDialog(It.IsAny<Dialog>()))
            .Callback((Dialog d) => m_dialogs.Remove(d));


        m_mockMessageRepo
            .Setup(r => r.CreateMessage(It.IsAny<Message>()))
            .Callback((Message m) => m_messages.Add(m));

        m_mockMessageRepo
            .Setup(r => r.UpdateMessage(It.IsAny<Message>()))
            .Callback((Message m) =>
            {
                var message = m_messages.FirstOrDefault(msg => msg.ID == m.ID);
                if (message != null)
                {
                    // Обновите поля сообщения, согласно вашим требованиям
                    message.Content = m.Content; // Пример
                    message.SendTime = m.SendTime; // Пример
                }
            });

        m_mockMessageRepo
            .Setup(r => r.DeleteMessage(It.IsAny<Message>()))
            .Callback((Message m) => m_messages.Remove(m));

        m_mockMessageRepo
            .Setup(r => r.GetMessageByID(It.IsAny<Guid>()))
            .ReturnsAsync((Guid guid) => m_messages.FirstOrDefault(m => m.ID == guid));

        m_mockMessageRepo
            .Setup(r => r.GetUserMessagesInDialog(It.IsAny<User>(), It.IsAny<Dialog>()))
            .ReturnsAsync((User user, Dialog dialog) =>
                m_messages.Where(m => m.SenderID == user.ID && m.DialogID == dialog.ID).ToList());

        m_mockMessageRepo
            .Setup(r => r.GetAllMessagesInDialog(It.IsAny<Dialog>()))
            .ReturnsAsync((Dialog dialog) =>
                m_messages.Where(m => m.DialogID == dialog.ID).ToList());

        m_mockMessageRepo
            .Setup(r => r.GetAllMessages())
            .ReturnsAsync(() => m_messages.ToList());
    }
}
