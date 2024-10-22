
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BasedGram.Database.Npgsql.Models;

public class MessageDbModel
{
    [Key]
    [Column("id")]
    public Guid ID { get; set; }

    [ForeignKey("Sender")]
    [Column("sender_id")]
    public Guid SenderID { get; set; }

    [ForeignKey("Dialog")]
    [Column("dialog_id")]
    public Guid DialogID { get; set; }

    [Column("sendtime")]
    public DateTime SentTime { get; set; }

    [Column("content", TypeName = "text")]
    public string? Content { get; set; }

    [Column("is_read")]
    public bool isReadFlag { get; set; }

    [Column("reaction_state")]
    public int ReactionState { get; set; }

    [Column("embedding")]
    public string? Embedding { get; set; }

    public MessageDbModel(Guid id,
                            Guid sender_id,
                            Guid dialog_id,
                            DateTime sendtime,
                            string? content,
                            bool is_read_fl,
                            int reaction_state,
                            string embedding)
    {
        ID = id;
        SenderID = sender_id;
        DialogID = dialog_id;
        SentTime = sendtime;
        Content = content;
        isReadFlag = is_read_fl;
        ReactionState = reaction_state;
        Embedding = embedding;
    }
    public MessageDbModel()
    {

    }
}