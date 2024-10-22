using Abp.Collections.Extensions;
using Asp.Versioning;
using BasedGram.Common.Core;
using BasedGram.Services.DialogService;
using BasedGram.Services.UserService;
using BasedGram.WebUI.DTO;
using BasedGram.WebUI.DTO.v2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BasedGram.WebUI.Controllers.v2;


[ApiController]
[ApiVersion(2)]
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

    [HttpGet("{dialog_id:guid}")]
    [Authorize]
    [SwaggerResponse(200, "Диалог", typeof(DialogDTOv2)), SwaggerResponse(401, "Unauthorized"), SwaggerResponse(403, "Forbidden (не свои диалоги)"), SwaggerResponse(404, "Not found")]
    public async Task<IActionResult> GetDialogByIdHandler(Guid dialog_id)
    {
        var user = await m_userService.GetUser(Guid.Parse(HttpContext.Items["userId"]!.ToString()!));

        var dial = await m_dialogService.GetDialog(dialog_id);
        if (dial is null)
        {
            return StatusCode(StatusCodes.Status404NotFound);
        }

        if (user.ID != dial.CreatorID && user.ID != dial.TargetID)
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        return new JsonResult(new DialogDTOv2(dial.ID, dial.IsBlockedFlag, dial.CreatorID, dial.TargetID));
    }

    [HttpPatch("{dialog_id:guid}")]
    [Authorize]
    [SwaggerResponse(200, "Диалог", typeof(DialogDTOv2)), SwaggerResponse(401, "Unauthorized"), SwaggerResponse(403, "Forbidden (не свои диалоги)"), SwaggerResponse(404, "Not found")]
    public async Task<IActionResult> PatchDialogByIDHandler(Guid dialog_id, [FromBody] DialogDTOv2 dialogDTOv2)
    {
        var user = await m_userService.GetUser(Guid.Parse(HttpContext.Items["userId"]!.ToString()!));

        var dial = await m_dialogService.GetDialog(dialog_id);
        if (dial is null)
        {
            return StatusCode(StatusCodes.Status404NotFound);
        }

        if (user.ID != dial.CreatorID && user.ID != dial.TargetID)
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        if (dialogDTOv2.is_blocked)
        {
            await m_dialogService.BlockDialog(dial);
        }
        else
        {
            await m_dialogService.UnblockDialog(dial);
        }
        dial = await m_dialogService.GetDialog(dialog_id);

        return new JsonResult(new DialogDTOv2(dial.ID, dial.IsBlockedFlag, dial.CreatorID, dial.TargetID));
    }

    [HttpGet("{dialog_id:guid}/messages")]
    [Authorize]
    [SwaggerResponse(200, "Массив сообщений", typeof(MessageDTOv2[])), SwaggerResponse(401, "Unauthorized"), SwaggerResponse(403, "Forbidden (не свои диалоги)"), SwaggerResponse(404, "Not found")]
    public async Task<IActionResult> GetDialogMessagesHandler(Guid dialog_id)
    {
        var user = await m_userService.GetUser(Guid.Parse(HttpContext.Items["userId"]!.ToString()!));

        var dial = await m_dialogService.GetDialog(dialog_id);
        if (dial is null)
        {
            return StatusCode(StatusCodes.Status404NotFound);
        }

        if (user.ID != dial.CreatorID && user.ID != dial.TargetID)
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        var msgs = await m_dialogService.GetAllMessages(dial);
        return new JsonResult(msgs.Select(m => new MessageDTOv2(
            m.ID,
            m.SenderID,
            m.DialogID,
            m.SendTime,
            m.Content,
            m.isReadFlag,
            0,
            0,
            m.Embedding
        )));
    }

    [HttpPost("{dialog_id:guid}/messages")]
    [Authorize]
    [SwaggerResponse(200, "Сообщение", typeof(MessageDTOv2)), SwaggerResponse(401, "Unauthorized"), SwaggerResponse(403, "Forbidden (не свои диалоги)"), SwaggerResponse(404, "Not found")]
    public async Task<IActionResult> PostDialogMessagesHandler(Guid dialog_id, [FromBody] MessageDTOv2 messageDTOv2)
    {
        var user = await m_userService.GetUser(Guid.Parse(HttpContext.Items["userId"]!.ToString()!));

        var dial = await m_dialogService.GetDialog(dialog_id);
        if (dial is null)
        {
            return StatusCode(StatusCodes.Status404NotFound);
        }

        if (user.ID != dial.CreatorID && user.ID != dial.TargetID)
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }
        try
        {
            var result = await m_dialogService.CreateMessage(user, dial, messageDTOv2.content, messageDTOv2.embedding);
            return new JsonResult(new MessageDTOv2(
                result.ID,
                result.SenderID,
                result.DialogID,
                result.SendTime,
                result.Content,
                result.isReadFlag,
                0,
                0,
                result.Embedding
            ));
        }
        catch (System.Exception)
        {
            return StatusCode(StatusCodes.Status400BadRequest);
        }
    }

    [HttpGet("{dialog_id:guid}/messages/{message_id:guid}")]
    [Authorize]
    [SwaggerResponse(200, "Сообщение", typeof(MessageDTOv2)), SwaggerResponse(401, "Unauthorized"), SwaggerResponse(403, "Forbidden (не свои диалоги)"), SwaggerResponse(404, "Not found")]
    public async Task<IActionResult> GetDialogMessageByIDHandler(Guid dialog_id, Guid message_id)
    {
        var user = await m_userService.GetUser(Guid.Parse(HttpContext.Items["userId"]!.ToString()!));

        var dial = await m_dialogService.GetDialog(dialog_id);
        if (dial is null)
        {
            return StatusCode(StatusCodes.Status404NotFound);
        }

        if (user.ID != dial.CreatorID && user.ID != dial.TargetID)
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        try
        {
            var result_arr = await m_dialogService.GetDialogMessages(user, dial);
            result_arr.RemoveAll(msg => msg.ID != message_id);
            if (result_arr.IsNullOrEmpty())
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            var result = result_arr[0];
            return new JsonResult(new MessageDTOv2(
                result.ID,
                result.SenderID,
                result.DialogID,
                result.SendTime,
                result.Content,
                result.isReadFlag,
                0,
                0,
                result.Embedding
            ));
        }
        catch (System.Exception)
        {
            return StatusCode(StatusCodes.Status400BadRequest);
        }
    }

    [HttpPatch("{dialog_id:guid}/messages/{message_id:guid}")]
    [Authorize]
    [SwaggerResponse(200, "Сообщение", typeof(MessageDTOv2)), SwaggerResponse(401, "Unauthorized"), SwaggerResponse(403, "Forbidden (не свои диалоги)"), SwaggerResponse(404, "Not found")]
    public async Task<IActionResult> PatchDialogMessageByIDHandler(Guid dialog_id, Guid message_id, [FromBody] MessageDTOv2 messageDTOv2)
    {
        var user = await m_userService.GetUser(Guid.Parse(HttpContext.Items["userId"]!.ToString()!));

        var dial = await m_dialogService.GetDialog(dialog_id);
        if (dial is null)
        {
            return StatusCode(StatusCodes.Status404NotFound);
        }

        if (user.ID != dial.CreatorID && user.ID != dial.TargetID)
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        try
        {
            var result_arr = await m_dialogService.GetDialogMessages(user, dial);
            result_arr.RemoveAll(msg => msg.ID != message_id);
            if (result_arr.IsNullOrEmpty())
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            var result = result_arr[0];

            result.Content = messageDTOv2.content;
            result.Embedding = messageDTOv2.embedding;
            result.isReadFlag = messageDTOv2.is_read;

            await m_dialogService.UpdateMessage(result);

            result_arr = await m_dialogService.GetDialogMessages(user, dial);
            result_arr.RemoveAll(msg => msg.ID != message_id);
            result = result_arr[0];
            return new JsonResult(new MessageDTOv2(
                result.ID,
                result.SenderID,
                result.DialogID,
                result.SendTime,
                result.Content,
                result.isReadFlag,
                0,
                0,
                result.Embedding
            ));
        }
        catch (System.Exception)
        {
            return StatusCode(StatusCodes.Status400BadRequest);
        }
    }
}
