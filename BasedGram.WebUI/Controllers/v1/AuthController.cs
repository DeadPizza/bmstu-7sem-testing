using Microsoft.AspNetCore.Mvc;
using BasedGram.WebUI.DTO.v1;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Authorization;
using BasedGram.WebUI.Utils;
using BasedGram.Services.UserService;
using BasedGram.Common.Core;
using BasedGram.Services.AuthService.Exceptions;
using BasedGram.Services.AuthService;
using Asp.Versioning;

namespace BasedGram.WebUI.Controllers.v1;

[ApiController]
[ApiVersion(1)]
[Route("api/v{v:apiVersion}/[controller]")]
public class AuthController : ControllerBase
{
    public readonly IUserService m_userService;
    public readonly IAuthService m_authService;
    private readonly JwtProvider m_jwtProvider;

    public AuthController(IUserService userService, IAuthService authService, JwtProvider jwtProvider)
    {
        m_userService = userService;
        m_authService = authService;
        m_jwtProvider = jwtProvider;
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterDTO data)
    {
        if (data.login == null || data.login.Length == 0
        || data.password == null || data.password.Length == 0
        || data.password_repeat == null || data.password_repeat.Length == 0)
        {
            return StatusCode(401);
        }

        if (data.login.All(p => p.Equals(' ')) || data.password.All(p => p.Equals(' ')) || data.password_repeat.All(p => p.Equals(' ')))
        {
            return StatusCode(401);
        }


        if(data.password != data.password_repeat)
        {
            return StatusCode(401);
        }
        // Console.WriteLine($"{data.login}, {data.password}, {data.password_repeat}");

        // if(data.password != data.password_repeat)
        // {
        //     return StatusCode("")
        // }

        User user;

        try
        {
            // Console.WriteLine(m_authService);

            await m_authService.RegisterUser(data.login!, data.password!);
            user = await m_authService.LoginUser(data.login!, data.password!);
        }
        catch (UserRegisterAlreadyExistsException)
        {
            return StatusCode(409, "User already exists");
        }

        // m_authService
        var jwtToken = m_jwtProvider.GenerateToken(user);
        HttpContext.Response.Cookies.Append(
            "access-token",
            jwtToken,
            new CookieOptions { Expires = DateTime.Now.AddHours(12) });

        return Ok(jwtToken);
    }

    [HttpPatch]
    public async Task<IActionResult> Login([FromBody] LoginDTO data)
    {
        User user;
        try
        {
            user = await m_authService.LoginUser(data.login!, data.password!);
        }
        catch (UserLoginNotFoundException)
        {
            return StatusCode(409, "User doesn't exists");
        }
        catch (IncorrectPasswordException)
        {
            return StatusCode(401, "Wrong password");
        }

        var jwtToken = m_jwtProvider.GenerateToken(user);
        HttpContext.Response.Cookies.Append(
            "access-token",
            jwtToken,
            new CookieOptions { Expires = DateTime.Now.AddHours(12) });

        return Ok(jwtToken);
    }
}
