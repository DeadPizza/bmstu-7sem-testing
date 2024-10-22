using Abp.Collections.Extensions;
using Asp.Versioning;
using BasedGram.Common.Core;
using BasedGram.Common.Enums;
using BasedGram.Services.DialogService;
using BasedGram.Services.UserService;
using BasedGram.WebUI.DTO.v2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BasedGram.WebUI.Controllers.v2;


[ApiController]
[ApiVersion(2)]
[Route("api/v{v:apiVersion}/[controller]")]
public class UsersController : ControllerBase
{
    public readonly IUserService m_userService;
    public readonly IDialogService m_dialogService;

    public UsersController(IUserService userService, IDialogService dialogService)
    {
        m_userService = userService;
        m_dialogService = dialogService;
    }

    [HttpGet]
    [Authorize]
    [SwaggerResponse(200, "Список пользователей", typeof(UserDTOv2[])), SwaggerResponse(401, "Unauthorized")]
    public async Task<IActionResult> GetAllUsersHandler([FromQuery] string? prefix)
    {
        var userlist = await m_userService.ListAllUsers();
        if (prefix is not null && !prefix.IsNullOrEmpty())
        {
            userlist.RemoveAll(a => !a.Login.StartsWith(prefix));
        }
        return new JsonResult(userlist.Select(u => new UserDTOv2(u.ID, u.Login, u.Role == Role.Admin, u.IsFreezed)));
    }

    [HttpGet("{user_id:guid}")]
    [Authorize]
    [SwaggerResponse(200, "Пользователь", typeof(UserDTOv2)), SwaggerResponse(401, "Unauthorized"), SwaggerResponse(404, "Not found")]
    public async Task<IActionResult> GetUserHandler(Guid user_id)
    {
        User user;
        user = await m_userService.GetUser(user_id);
        if (user is null)
        {
            return StatusCode(StatusCodes.Status404NotFound);
        }
        return new JsonResult(new UserDTOv2(user.ID, user.Login, user.Role == Role.Admin, user.IsFreezed));
    }

    [HttpPatch("{user_id:guid}")]
    [Authorize]
    [SwaggerResponse(200, "Измененный пользователь", typeof(UserDTOv2)), SwaggerResponse(401, "Unauthorized"), SwaggerResponse(403, "Forbidden"), SwaggerResponse(404, "Not found")]
    // TODO: мощнейший
    public async Task<IActionResult> PatchUserHandler(Guid user_id, [FromBody] UserDTOv2 userDTOv2)
    {
        {
            var requester = await m_userService.GetUser(Guid.Parse(HttpContext.Items["userId"]!.ToString()!));
            if (requester.Role != Role.Admin)
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }
        }

        User user;
        user = await m_userService.GetUser(user_id);
        if (user is null)
        {
            return StatusCode(StatusCodes.Status404NotFound);
        }

        if (user.IsFreezed != userDTOv2.is_freezed)
        {
            await m_userService.FreezeUser(user);
        }
        user = await m_userService.GetUser(user_id);
        return new JsonResult(new UserDTOv2(user.ID, user.Login, user.Role == Role.Admin, user.IsFreezed));
    }

    [HttpDelete("{user_id:guid}")]
    [Authorize]
    [SwaggerResponse(200, "Удаленный пользователь"), SwaggerResponse(401, "Unauthorized"), SwaggerResponse(403, "Forbidden"), SwaggerResponse(404, "Not found")]
    public async Task<IActionResult> DeleteUserHandler(Guid user_id)
    {
        var requester = await m_userService.GetUser(Guid.Parse(HttpContext.Items["userId"]!.ToString()!));
        if (requester.Role != Role.Admin)
        {
            return StatusCode(StatusCodes.Status403Forbidden);

        }
        var found = await m_userService.GetUser(user_id);
        if (found is null)
        {
            return StatusCode(StatusCodes.Status404NotFound);
        }
        await m_userService.EraseUser(found);
        return Ok();
    }

    [HttpPost("{user_id:guid}/dialogs")]
    [Authorize]
    [SwaggerResponse(200, "Диалог создан успешно", typeof(DialogDTOv2)), SwaggerResponse(400, "Диалог уже существует"), SwaggerResponse(401, "Unauthorized"), SwaggerResponse(403, "Forbidden (user freezed)"), SwaggerResponse(404, "Not found")]
    public async Task<IActionResult> PostUserDialogsHandler(Guid user_id, [FromQuery] Guid target_id)
    {
        var user = await m_userService.GetUser(Guid.Parse(HttpContext.Items["userId"]!.ToString()!));
        if (user is null)
        {
            return StatusCode(StatusCodes.Status404NotFound);
        }
        if (user.ID != user_id || user.IsFreezed)
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }
        var targetuser = await m_userService.GetUser(target_id);
        if (targetuser is null)
        {
            return StatusCode(StatusCodes.Status404NotFound);
        }

        try
        {
            var new_dialog = await m_dialogService.CreateDialog(user, targetuser);
            if (new_dialog is null)
            {
                throw new Exception();
            }
            return new JsonResult(new DialogDTOv2(new_dialog.ID, new_dialog.IsBlockedFlag, new_dialog.CreatorID, new_dialog.TargetID));
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status400BadRequest);
        }
    }

    [HttpGet("{user_id:guid}/dialogs")]
    [Authorize]
    [SwaggerResponse(200, "Диалог получен успешно", typeof(DialogDTOv2[])), SwaggerResponse(401, "Unauthorized"), SwaggerResponse(403, "Forbidden (not users dials)"), SwaggerResponse(404, "Not found")]
    public async Task<IActionResult> GetUserDialogsHandler(Guid user_id)
    {
        var user = await m_userService.GetUser(Guid.Parse(HttpContext.Items["userId"]!.ToString()!));
        if(user is null)
        {
            return StatusCode(StatusCodes.Status404NotFound);
        }

        if (user.ID != user_id || user.IsFreezed)
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        var dials = await m_dialogService.GetUserDialogs(user);

        return new JsonResult(dials.Select(d => new DialogDTOv2(d.ID, d.IsBlockedFlag, d.CreatorID, d.TargetID)));
    }
}
