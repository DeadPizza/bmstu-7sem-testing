using Newtonsoft.Json;

namespace BasedGram.WebUI.DTO;

public class Login2FA_DTO
{
    public Login2FA_DTO()
    {
        Email = "";
        Password = "";
        VerificationCode = "";
    }

    public Login2FA_DTO(string email, string? password, string? verificationCode)
    {
        Email = email;
        Password = password;
        VerificationCode = verificationCode;
    }

    internal void Deconstruct(out string email, out string password, out string verificationCode)
    {
        email = Email;
        password = Password;
        verificationCode = VerificationCode;
    }

    [JsonProperty("email")]
    public string Email { get; set; }

    [JsonProperty("password")]
    public string? Password { get; set; }

    [JsonProperty("verificationCode")]
    public string? VerificationCode { get; set; }
}
