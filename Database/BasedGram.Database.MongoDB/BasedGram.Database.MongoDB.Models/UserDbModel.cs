using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson.Serialization.Attributes;

namespace BasedGram.Database.MongoDB.Models;

public class UserDbModel
{
    [Key]
    [Column("id")]
    [BsonId]
    public Guid ID { get; set; }

    [Column("login")]
    public string Login { get; set; }

    [Column("pass_hash")]
    public string PasswordHash { get; set; }

    [Column("role")]
    public int Role { get; set; }

    [Column("is_authorised")]
    public bool IsAuthorised { get; set; }

    [Column("is_freezed")]
    public bool IsFreezed { get; set; }


    public UserDbModel(Guid id, string login, string password_hash, int role, bool authorised_fl, bool is_freezed)
    {
        ID = id;
        Login = login;
        PasswordHash = password_hash;
        Role = role;
        IsAuthorised = authorised_fl;
        IsFreezed = is_freezed;
    }

    public UserDbModel()
    {
    }
}
