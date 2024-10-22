using Microsoft.AspNetCore.Mvc;
using BasedGram.WebUI.DTO;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Authorization;
using BasedGram.WebUI.Utils;
using BasedGram.Services.UserService;
using BasedGram.Common.Core;
using BasedGram.Services.AuthService.Exceptions;
using BasedGram.Services.AuthService;
using Asp.Versioning;
using Swashbuckle.AspNetCore.Annotations;
using BasedGram.WebUI.DTO.v2;

namespace BasedGram.WebUI.Controllers.v2;

[ApiController]
[ApiVersion(2)]
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
    [EndpointSummary("Регистрация")]
    [SwaggerResponse(200, "Успешная регистрация", typeof(JwtTokenResponseDTOv2)), SwaggerResponse(400, "Недопустимый ввод"), SwaggerResponse(409, "Такой пользователь уже существует")]
    public async Task<IActionResult> RegisterHandler([FromBody] AuthDTOv2 authDTOv2)
    {
        if (authDTOv2.login.All(p => p.Equals(' ')) || authDTOv2.password.All(p => p.Equals(' ')))
        {
            return StatusCode(StatusCodes.Status400BadRequest);
        }

        User user;
        try
        {
            await m_authService.RegisterUser(authDTOv2.login, authDTOv2.password);
            user = await m_authService.LoginUser(authDTOv2.login, authDTOv2.password);
        }
        catch (UserRegisterAlreadyExistsException)
        {
            return StatusCode(StatusCodes.Status409Conflict, "User already exists");
        }

        var jwtToken = m_jwtProvider.GenerateToken(user);
        HttpContext.Response.Cookies.Append(
            "access-token",
            jwtToken,
            new CookieOptions { Expires = DateTime.Now.AddHours(12) });

        return new JsonResult(new JwtTokenResponseDTOv2(jwtToken));
    }

    [HttpPatch]
    [EndpointSummary("Авторизация")]
    [SwaggerResponse(200, "Успешная авторизация", typeof(JwtTokenResponseDTOv2)), SwaggerResponse(400, "Некорректные данные для входа")]
    public async Task<IActionResult> LoginHandler([FromBody] AuthDTOv2 authDTOv2)
    {
        User user;
        try
        {
            user = await m_authService.LoginUser(authDTOv2.login, authDTOv2.password);
        }
        catch (UserRegisterAlreadyExistsException)
        {
            return StatusCode(StatusCodes.Status400BadRequest, "Invalid data");
        }

        var jwtToken = m_jwtProvider.GenerateToken(user);
        HttpContext.Response.Cookies.Append(
            "access-token",
            jwtToken,
            new CookieOptions { Expires = DateTime.Now.AddHours(12) });

        return new JsonResult(new JwtTokenResponseDTOv2(jwtToken));
    }
}
