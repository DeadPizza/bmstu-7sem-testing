namespace BasedGram.Common.Core;
using System.Text.Json.Serialization;

public class Dialog
{
    [JsonConstructor]
    public Dialog() {}
    public Guid ID { get; set; }
    public bool IsBlockedFlag { get; set; }
    public Guid CreatorID { get; set; }
    public Guid TargetID { get; set; }

    public Dialog(Guid id, bool is_blocked, Guid creator, Guid target)
    {
        ID = id;
        IsBlockedFlag = is_blocked;
        CreatorID = creator;
        TargetID = target;
    }
}