using BasedGram.Database.Npgsql.Models;

namespace BasedGram.Tests.Common.DataBuilders.Db;

public class UserDbModelBuilder
{
        private UserDbModel _userDbModel = new();

    public UserDbModelBuilder WithID(Guid id)
    {
        _userDbModel.ID = id;
        return this;
    }

    public UserDbModelBuilder WithLogin(string login)
    {
        _userDbModel.Login = login;
        return this;
    }

    public UserDbModelBuilder WithPasswordHash(string passwordHash)
    {
        _userDbModel.PasswordHash = passwordHash;
        return this;
    }

    public UserDbModelBuilder WithRole(int role)
    {
        _userDbModel.Role = role;
        return this;
    }

    public UserDbModelBuilder WithIsAuthorised(bool isAuthorised)
    {
        _userDbModel.IsAuthorised = isAuthorised;
        return this;
    }

    public UserDbModelBuilder WithIsFreezed(bool isFreezed)
    {
        _userDbModel.IsFreezed = isFreezed;
        return this;
    }

    public UserDbModel Build()
    {
        var userDbModel = _userDbModel;
        _userDbModel = new UserDbModel(); // Сбрасываем текущее состояние после вызова Build
        return userDbModel;
    }

    // Метод Copy для возможности клонирования с изменениями
    public UserDbModelBuilder Copy(UserDbModel userDbModel)
    {
        _userDbModel = new UserDbModel(
            userDbModel.ID,
            userDbModel.Login,
            userDbModel.PasswordHash,
            userDbModel.Role,
            userDbModel.IsAuthorised,
            userDbModel.IsFreezed
        );
        return this;
    }
}
