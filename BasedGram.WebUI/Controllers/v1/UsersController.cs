using Asp.Versioning;
using BasedGram.Common.Enums;
using BasedGram.Services.DialogService;
using BasedGram.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BasedGram.WebUI.Controllers.v1;

[ApiController]
[ApiVersion(1)]
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

    [Authorize]
    [HttpGet()]
    public async Task<IActionResult> GetAllProfiles()
    {
        var userlist = await m_userService.ListAllUsers();
        return new JsonResult(userlist.Select(elem => new
        {
            id = elem.ID,
            login = elem.Login,
            isAdmin = elem.Role == Role.Admin,
            isFreezed = elem.IsFreezed
        }));
    }

    [Authorize]
    [HttpGet("{user_id:guid}")]
    public async Task<IActionResult> GetMyProfile(Guid user_id)
    {
        if (user_id == Guid.Empty)
        {
            user_id = Guid.Parse(HttpContext.Items["userId"]!.ToString()!);
        }
        var user = await m_userService.GetUser(user_id);
        // var user = await m_userService.GetUser(Guid.Parse(HttpContext.Items["userId"]!.ToString()!));
        return new JsonResult(new
        {
            id = user.ID,
            login = user.Login,
            isAdmin = user.Role == Common.Enums.Role.Admin,
            isFreezed = user.IsFreezed
        });
    }

    [Authorize]
    [HttpPut("{user_id:guid}")]
    public async Task<IActionResult> FreezeProfile(Guid user_id)
    {
        var requester = await m_userService.GetUser(Guid.Parse(HttpContext.Items["userId"]!.ToString()!));
        if (requester.Role != Role.Admin)
        {
            return StatusCode(403);
        }
        await m_userService.FreezeUser(await m_userService.GetUser(user_id));
        return new JsonResult(new { (await m_userService.GetUser(user_id)).IsFreezed });
    }

    [Authorize]
    [HttpDelete("{user_id:guid}")]
    public async Task<IActionResult> DeleteProfile(Guid user_id)
    {
        var requester = await m_userService.GetUser(Guid.Parse(HttpContext.Items["userId"]!.ToString()!));
        if (requester.Role != Role.Admin)
        {
            return StatusCode(403);
        }
        await m_userService.EraseUser(await m_userService.GetUser(user_id));
        return Ok();
    }

    // [Authorize]
    // [HttpGet("{user_id:guid}/dialogs")]
    // public async Task<IActionResult> GetMyDialogs(Guid user_id)
    // {
    //     var user = await m_userService.GetUser(Guid.Parse(HttpContext.Items["userId"]!.ToString()!));
    //     var dials = await m_dialogService.GetUserDialogs(user);
    //     return new JsonResult(dials);
    // }

    [Authorize]
    [HttpPost("{user_id:guid}/dialogs")]
    public async Task<IActionResult> CreateNewDialog(Guid user_id,  [FromQuery] Guid other_user_id)
    {
        var user = await m_userService.GetUser(Guid.Parse(HttpContext.Items["userId"]!.ToString()!));
        if (user.ID != user_id)
        {
            return StatusCode(403);
        }

        var targetuser = await m_userService.GetUser(other_user_id);
        return Ok((await m_dialogService.CreateDialog(user, targetuser)).ID);
    }

    [Authorize]
    // [HttpGet("{user_id:guid}/dialogs/{other_user_id:guid}")]
    [HttpGet("{user_id:guid}/dialogs")]
    public async Task<IActionResult> GetMyConcreteDialog(Guid user_id, [FromQuery] Guid other_user_id)
    {
        var user = await m_userService.GetUser(Guid.Parse(HttpContext.Items["userId"]!.ToString()!));
        if (user.ID != user_id && user.ID != other_user_id)
        {
            return StatusCode(403);
        }

        var dials = await m_dialogService.GetUserDialogs(user);
        if (other_user_id == Guid.Empty)
        {
            return new JsonResult(dials);
        }

        var dial = dials.Where(u => u.CreatorID == other_user_id || u.TargetID == other_user_id).ToList().First();

        if (dial is null)
        {
            return StatusCode(404);
        }

        return new JsonResult(dial);
    }
}
