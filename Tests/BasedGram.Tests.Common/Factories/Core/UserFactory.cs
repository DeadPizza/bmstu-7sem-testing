using BasedGram.Common.Core;
using BasedGram.Common.Enums;

namespace BasedGram.Tests.Common.Factories.Core;

public class UserFactory
{
    public static User Create(
        Guid id,
        String login,
        String passwordHash,
        Role role = Role.User,
        bool authFlag = false,
        bool isFreezed = false)
    {
        return new User(id, login, passwordHash, role, authFlag, isFreezed);
    }

    public static User Copy(User other)
    {
        return new User(
            other.ID,
            other.Login,
            other.PasswordHash,
            other.Role,
            other.AuthFlag,
            other.IsFreezed
        );
    }

    public static User CreateEmpty()
    {
        return new User();
    }
}