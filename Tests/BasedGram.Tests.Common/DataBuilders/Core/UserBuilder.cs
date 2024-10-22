using BasedGram.Common.Core;
using BasedGram.Common.Enums;

namespace BasedGram.Tests.Common.DataBuilders.Core;

public class UserBuilder
{
    private User _user = new();

    public UserBuilder WithID(Guid id)
    {
        _user.ID = id;
        return this;
    }

    public UserBuilder WithLogin(string login)
    {
        _user.Login = login;
        return this;
    }

    public UserBuilder WithPasswordHash(string passwordHash)
    {
        _user.PasswordHash = passwordHash;
        return this;
    }

    public UserBuilder WithRole(Role role)
    {
        _user.Role = role;
        return this;
    }

    public UserBuilder WithAuthFlag(bool authFlag)
    {
        _user.AuthFlag = authFlag;
        return this;
    }

    public UserBuilder WithIsFreezed(bool isFreezed)
    {
        _user.IsFreezed = isFreezed;
        return this;
    }

    public User Build()
    {
        var user = _user;
        _user = new User(); // Сбрасываем текущее состояние после вызова Build
        return user;
    }

    // Метод Copy для возможности клонирования с изменениями
    public UserBuilder Copy(User user)
    {
        _user = new User(user.ID, user.Login, user.PasswordHash, user.Role, user.AuthFlag, user.IsFreezed);
        return this;
    }
}
