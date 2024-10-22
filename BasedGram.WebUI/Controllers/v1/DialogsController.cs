using Asp.Versioning;
using BasedGram.Common.Core;
using BasedGram.Services.DialogService;
using BasedGram.Services.UserService;
using BasedGram.WebUI.DTO.v1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BasedGram.WebUI.Controllers.v1;


[ApiController]
[ApiVersion(1)]
[Route("api/v{v:apiVersion}/[controller]")]
public class DialogsController : ControllerBase
{
    public readonly IUserService m_userService;
    public readonly IDialogService m_dialogService;
    public DialogsController(IUserService userService, IDialogService dialogService)
    {
        m_userService = userService;
        m_dialogService = dialogService;
    }

    [Authorize]
    [HttpGet("{dialog_id:guid}")]
    public async Task<IActionResult> GetDialogById(Guid dialog_id)
    {
        var user = await m_userService.GetUser(Guid.Parse(HttpContext.Items["userId"]!.ToString()!));

        var dial = await m_dialogService.GetDialog(dialog_id);

        if (user.ID != dial.CreatorID && user.ID != dial.TargetID)
        {
            return StatusCode(403);
        }
        
        return new JsonResult(dial);
    }

    [Authorize]
    [HttpPatch("{dialog_id:guid}")]
    public async Task<IActionResult> MarkTargetAsRead(Guid dialog_id, DialDTO dialDTO)
    {
        var user = await m_userService.GetUser(Guid.Parse(HttpContext.Items["userId"]!.ToString()!));
        var dial = await m_dialogService.GetDialog(dialog_id);

        var target_user_id = dial.CreatorID == user.ID ? dial.TargetID : dial.CreatorID;

        if (dialDTO.markAsRead)
        {
            foreach (var msg in await m_dialogService.GetDialogMessages(await m_userService.GetUser(target_user_id), dial))
            {
                if (!msg.isReadFlag)
                {
                    msg.isReadFlag = true;
                    await m_dialogService.UpdateMessage(msg);
                }
            }
        }
        if (dialDTO.toggleBlock)
        {
            if (dial.IsBlockedFlag)
            {
                await m_dialogService.UnblockDialog(dial);
            }
            else
            {
                await m_dialogService.BlockDialog(dial);
            }
        }
        return new JsonResult(new {isBlocked = dialDTO.toggleBlock ? !dial.IsBlockedFlag : dial.IsBlockedFlag});
    }

    [Authorize]
    [HttpPost("{dialog_id:guid}/messages")]
    public async Task<IActionResult> MakeDialogMessages(Guid dialog_id, [FromBody] MessageDTO msg)
    {
        var user = await m_userService.GetUser(Guid.Parse(HttpContext.Items["userId"]!.ToString()!));
        var dialog = await m_dialogService.GetDialog(dialog_id);
        
        if (dialog.CreatorID != user.ID && dialog.TargetID != user.ID)
        {
            return StatusCode(403);
        }

        if(dialog.IsBlockedFlag)
        {
            return StatusCode(403);
        }

        await m_dialogService.CreateMessage(user, dialog, msg.Content!, msg.Embedding!);
        return Ok();
        // var dials = await m_dialogService.GetUserDialogs(user);
        // return new JsonResult(dials);
    }

    [Authorize]
    [HttpGet("{dialog_id:guid}/messages")]
    public async Task<IActionResult> GetDialogMessages(Guid dialog_id)
    {
        var user = await m_userService.GetUser(Guid.Parse(HttpContext.Items["userId"]!.ToString()!));
        var dialog = await m_dialogService.GetDialog(dialog_id);
        if (dialog.CreatorID != user.ID && dialog.TargetID != user.ID)
        {
            return StatusCode(403);
        }

        // Console.WriteLine(dialog.ID);
        return new JsonResult(await m_dialogService.GetAllMessages(dialog));
        // var dials = await m_dialogService.GetUserDialogs(user);
        // return new JsonResult(dials);
    }

    [Authorize]
    [HttpPut("{dialog_id:guid}/messages/{message_id:guid}")]
    public async Task<IActionResult> EditMessage(Guid dialog_id, Guid message_id, [FromBody] MessageDTO msg)
    {
        var user = await m_userService.GetUser(Guid.Parse(HttpContext.Items["userId"]!.ToString()!));
        var dialog = await m_dialogService.GetDialog(dialog_id);
        if (dialog.CreatorID != user.ID && dialog.TargetID != user.ID)
        {
            return StatusCode(403);
        }

        var message = (await m_dialogService.GetAllMessages(dialog)).Find(elem => elem.ID == message_id);

        if (msg.Content is not null)
        {
            message.Content = msg.Content;
        }
        if (msg.isReadFlag is not null)
        {
            message.isReadFlag = (bool)msg.isReadFlag;
        }
        if (msg.ReactionState is not null)
        {
            message.ReactionState = (Reaction)msg.ReactionState!;
        }

        await m_dialogService.UpdateMessage(message);

        return Ok();
    }
}
