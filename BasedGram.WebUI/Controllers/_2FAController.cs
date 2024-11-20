using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using BasedGram.WebUI.DTO;
using Microsoft.AspNetCore.Mvc;

namespace BasedGram.WebUI.Controllers;
class UserData
{
    public Dictionary<string, string> Codes = new Dictionary<string, string>();
    public string UserPassword = "1337";
    public string UserEmail = Environment.GetEnvironmentVariable("USER_EMAIL")!;
};

[ApiController]
[Route("api/[controller]")]
public class _2FAController : ControllerBase
{
    private static readonly UserData userData = new();

    private static void SendEmail(string data)
    {
        string to = userData.UserEmail;
        string from = Environment.GetEnvironmentVariable("SENDER_EMAIL")!;
        MailMessage message =
            new(from, to) { Subject = "Your verification code.", Body = $"Your code is {data}." };
        var client = new SmtpClient(Environment.GetEnvironmentVariable("SMTP_HOST"))
        {
            Port = 587,
            Credentials = new NetworkCredential(
                userData.UserEmail,
                Environment.GetEnvironmentVariable("EMAIL_PASSWORD")
            ),
            EnableSsl = true,
        };

        try
        {
            client.Send(message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception caught in SendMail(): {0}", ex.ToString());
        }
    }

    private static string GetVerificationCode()
    {
        string? code = Environment.GetEnvironmentVariable("VERIFICATION_CODE");
        code ??= RandomNumberGenerator.GetString(
            new ReadOnlySpan<char>(['0', '1', '2', '3', '4', '5', '6', '7', '8', '9']),
            6
        );
        return code!;
    }

    [HttpGet("status")]
    public ActionResult Status()
    {
        return Ok();
    }

    [HttpPost("login")]
    public ActionResult Login([FromBody] Login2FA_DTO loginDto)
    {
        try
        {
            if (
                loginDto.Email == userData.UserEmail
                && loginDto.Password == userData.UserPassword
            )
            {
                string code = GetVerificationCode();
                SendEmail(code);
                userData.Codes[loginDto.Email] = code;

                return Ok("Verification code sent to email.");
            }
            return Unauthorized("Invalid credentials.");
        }
        catch (Exception e)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                $"Unexpected server error: {e.Message}."
            );
        }
    }

    [HttpPost("login/verify")]
    public ActionResult VerifyLogin([FromBody] Login2FA_DTO loginVerifyDto)
    {
        try
        {
            var (email, pass, code) = loginVerifyDto;
            foreach (var key in userData.Codes.Keys)
            {
                Console.WriteLine(key);
            }

            if (userData.Codes.ContainsKey(email) && userData.Codes[email] == code)
            {
                userData.Codes.Remove(loginVerifyDto.Email);
                return Ok("Account verified successfully.");
            }
            return Unauthorized("Invalid credentials.");
        }
        catch (Exception e)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                $"Unexpected server error: {e.Message}."
            );
        }
    }

    [HttpPost("reset")]
    public ActionResult ResetPassword([FromBody] Login2FA_DTO resetDto)
    {
        try
        {
            if (
                resetDto.Email == userData.UserEmail
                && resetDto.Password == userData.UserPassword
            )
            {
                string code = GetVerificationCode();
                SendEmail(code);
                userData.Codes.Add(resetDto.Email, code);

                return Ok("Verification code sent to email.");
            }
            return Unauthorized("Invalid credentials.");
        }
        catch (Exception e)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                $"Unexpected server error: {e.Message}."
            );
        }
    }

    [HttpPost("reset/verify")]
    public ActionResult ResetPasswordVerify([FromBody] Login2FA_DTO resetVerifyDto)
    {
        try
        {
            var (email, password, code) = resetVerifyDto;
            if (userData.Codes.ContainsKey(email) && userData.Codes[email] == code)
            {
                userData.UserPassword = resetVerifyDto.Password!;
                userData.Codes.Remove(resetVerifyDto.Email);
                return Ok("Password reset successfully.");
            }
            return Unauthorized("Invalid credentials.");
        }
        catch (Exception e)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                $"Unexpected server error: {e.Message}."
            );
        }
    }

}
