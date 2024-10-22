using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BasedGram.Common.Core;
using Microsoft.IdentityModel.Tokens;

namespace BasedGram.WebUI.Utils;

public class JwtProvider(IConfiguration configuration)
{
    readonly IConfiguration config = configuration;
    public string GenerateToken(User user)
    {
        Claim[] claims = [new("userId", user.ID.ToString())];

        // Console.WriteLine($"secret: {config["Jwt:Secret"]}, expires: {double.Parse(config["Jwt:ValidityInHours"]!)}");
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Secret"]!)),
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            claims: claims,
            signingCredentials: signingCredentials,
            expires: DateTime.UtcNow.AddHours(double.Parse(config["Jwt:ValidityInHours"]!))
        );

        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

        return tokenValue;
    }
}