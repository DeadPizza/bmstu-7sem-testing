using BasedGram.Database.Npgsql.Models;

namespace BasedGram.Tests.Common.Factories.Db;

public class UserDbModelFactory
{
    public static UserDbModel Create(
        Guid id,
        string login,
        string passwordHash,
        int role,
        bool isAuthorised,
        bool isFreezed)
    {
        return new UserDbModel(id, login, passwordHash, role, isAuthorised, isFreezed);
    }

    public static UserDbModel Copy(UserDbModel other)
    {
        return new UserDbModel(
            other.ID,
            other.Login,
            other.PasswordHash,
            other.Role,
            other.IsAuthorised,
            other.IsFreezed
        );
    }

    public static UserDbModel CreateEmpty()
    {
        return new UserDbModel();
    }
}