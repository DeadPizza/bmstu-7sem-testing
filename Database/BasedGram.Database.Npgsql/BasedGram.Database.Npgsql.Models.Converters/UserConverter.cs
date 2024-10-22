using System.Diagnostics.CodeAnalysis;
using BasedGram.Common.Core;
using BasedGram.Common.Enums;
using BasedGram.Database.Npgsql.Models;

namespace BasedGram.Database.Npgsql.Models.Converters;

public static class UserConverter
{
    [return: NotNullIfNotNull(nameof(model))]
    public static User? DbToCoreModel(UserDbModel? model)
    {
        if (model is null)
        {
            return null;
        }

        Role role = Role.User;
        switch (model.Role)
        {
            case 0: role = Role.User; break;
            case 1: role = Role.Admin; break;
        }

        return new(model.ID, model.Login, model.PasswordHash, role, model.IsAuthorised, model.IsFreezed);
    }

    [return: NotNullIfNotNull(nameof(model))]
    public static UserDbModel? CoreToDbModel(User? model)
    {
        if (model is null)
        {
            return null;
        }

        int role = 0;
        switch (model.Role)
        {
            case Role.User: role = 0; break;
            case Role.Admin: role = 1; break;
        }

        return new(
            model.ID,
            model.Login,
            model.PasswordHash,
            role,
            model.AuthFlag,
            model.IsFreezed
        );
    }
}