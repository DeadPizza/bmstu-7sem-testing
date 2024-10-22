using System.Security.Cryptography;
using BasedGram.Common.Enums;
using System.Text.Json.Serialization;

namespace BasedGram.Common.Core;

public class User
{
    [JsonConstructor]
    public User() { }
    public Guid ID { get; set; }
    public String Login { get; set; }
    public String PasswordHash { get; set; }
    public Role Role { get; set; }
    public bool AuthFlag { get; set; }
    public bool IsFreezed { get; set; }

    public User(Guid id, String login, String password, Role role = Role.User, bool auth_flag = false, bool IsFreezedv = false)
    {
        ID = id;
        Login = login;
        PasswordHash = password;
        Role = role;
        AuthFlag = auth_flag;
        IsFreezed = IsFreezedv;
    }
}