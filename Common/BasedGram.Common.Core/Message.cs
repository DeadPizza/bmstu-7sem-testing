using BasedGram.Common.Enums;
using System.Text.Json.Serialization;

namespace BasedGram.Common.Core;

public class Message
{
    public Guid ID { get; set; }
    public Guid SenderID { get; set; }
    public Guid DialogID { get; set; }
    public DateTime SendTime { get; set; }
    public string? Content { get; set; }
    public bool isReadFlag { get; set; }
    public Reaction ReactionState { get; set; }
    public string? Embedding { get; set; }

    public Message(Guid id, Guid sender_id, Guid dialog_id, DateTime send_time, string? content, bool is_read, Reaction reaction, string embedding)
    {
        ID = id;
        SenderID = sender_id;
        DialogID = dialog_id;
        SendTime = send_time;
        Content = content;
        isReadFlag = is_read;
        ReactionState = reaction;
        Embedding = embedding;
    }

    [JsonConstructor]
    public Message() { }
}