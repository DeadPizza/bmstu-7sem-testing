namespace BasedGram.WebUI.DTO.v2;

public class DialogDTOv2
{
    public DialogDTOv2(Guid id, bool is_blocked, Guid creator_id, Guid target_id)
    {
        this.id = id;
        this.is_blocked = is_blocked;
        this.creator_id = creator_id;
        this.target_id = target_id;
    }

    public Guid id {get;set;}
    public bool is_blocked {get;set;}
    public Guid creator_id {get;set;}
    public Guid target_id {get;set;}
}
