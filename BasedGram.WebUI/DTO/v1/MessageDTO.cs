namespace BasedGram.WebUI.DTO.v1;

public class MessageDTO
{
    public DateTime? SendTime { get; set; }
    public string? Content { get; set; }
    public bool? isReadFlag { get; set; }
    public int? ReactionState { get; set; }
    public string? Embedding { get; set; }
}