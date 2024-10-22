namespace BasedGram.WebUI.DTO.v2;

public class MessageDTOv2
{
    public MessageDTOv2(Guid id, Guid sender_id, Guid dialog_id, DateTime send_time, string content, bool is_read, int sender_reaction_state, int target_reaction_state, string embedding)
    {
        this.id = id;
        this.sender_id = sender_id;
        this.dialog_id = dialog_id;
        this.send_time = send_time;
        this.content = content;
        this.is_read = is_read;
        this.sender_reaction_state = sender_reaction_state;
        this.target_reaction_state = target_reaction_state;
        this.embedding = embedding;
    }

    public Guid id { get; set; }
    public Guid sender_id { get; set; }
    public Guid dialog_id { get; set; }
    public DateTime send_time { get; set; }
    public string content { get; set; }
    public bool is_read { get; set; }
    public int sender_reaction_state { get; set; }
    public int target_reaction_state { get; set; }
    public string embedding { get; set; }
}
