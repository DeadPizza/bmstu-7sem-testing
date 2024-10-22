namespace BasedGram.WebUI.DTO.v2;

public class UserDTOv2
{
    public UserDTOv2(Guid id, string login, bool is_admin, bool is_freezed)
    {
        this.id = id;
        this.login = login;
        this.is_admin = is_admin;
        this.is_freezed = is_freezed;
    }

    public Guid id { get; set; }
    public string login { get; set; }
    public bool is_admin { get; set; }
    public bool is_freezed { get; set; }
}
