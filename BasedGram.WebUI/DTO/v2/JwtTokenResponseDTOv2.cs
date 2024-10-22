namespace BasedGram.WebUI.DTO.v2;

public class JwtTokenResponseDTOv2
{
    public string jwt_token { get; set; }

    public JwtTokenResponseDTOv2(string jwt_token)
    {
        this.jwt_token = jwt_token;
    }
}
