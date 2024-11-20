using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson.Serialization.Attributes;
// using MewingPad.Database.MongoDB.Models;

namespace BasedGram.Database.MongoDB.Models;

public class DialogDbModel
{
    [Key]
    [Column("id")]
    [BsonId]
    public Guid ID { get; set; }

    [Column("is_blocked")]
    public bool IsBlockedFlag { get; set; }

    [Column("creator_id")]
    public Guid CreatorID { get; set; }

    [Column("collocutor_id")]
    public Guid ColocutorID { get; set; }

    public DialogDbModel(Guid id, bool is_blocked_fl, Guid creator_id, Guid collocutor_id)
    {
        ID = id;
        IsBlockedFlag = is_blocked_fl;
        CreatorID = creator_id;
        ColocutorID = collocutor_id;
    }

    public DialogDbModel()
    {
        // You can initialize properties here if needed
    }
}