using BasedGram.Common.Core;

namespace BasedGram.Database.Core.Repositories;

public interface IUserRepository
{
    Task CreateUser(User user);
    Task DeleteUser(User user);
    Task UpdateUser(User user);
    Task<List<User>> ListAllUsers();
    Task<List<User>> ListAllAdmins();
    Task<User?> GetUserByID(Guid guid);
}