using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Drawing;

namespace BasedGram.WebUI.Controllers.v2;


[ApiController]
[ApiVersion(2)]
[Route("api/v{v:apiVersion}/[controller]")]
public class ImagesController : ControllerBase
{
    [HttpGet("{image_id:int}")]
    [SwaggerResponse(200, "Success"), SwaggerResponse(401, "Unauthorized"), SwaggerResponse(404, "Image not found")]
    public IActionResult GetTestImage(int image_id)
    {
        try
        {
            var imagePath = $"/home/deadpizza/Рабочий стол/МГТУ/6 семестр/PPO/lab_09/BasedGram/BasedGram.WebUI/imgs/{image_id}.png"; // Replace with the actual path to test.png
            var image = System.IO.File.OpenRead(imagePath);
            return File(image, "image/png");
        }
        catch (System.Exception)
        {
            return StatusCode(StatusCodes.Status404NotFound);
        }
    }


    [HttpPost]
    [SwaggerResponse(200, "Success"), SwaggerResponse(401, "Unauthorized")]
    public async Task<IActionResult> UploadImage(IFormFile imageFile)
    {
        // imageFile.
        // Process the uploaded image, for example, save it to a specific location
        if (imageFile != null && imageFile.Length > 0)
        {
            var hash_code = imageFile.GetHashCode();

            var imagePath = $"/home/deadpizza/Рабочий стол/МГТУ/6 семестр/PPO/lab_09/BasedGram/BasedGram.WebUI/imgs/{hash_code}.png";
            // Set the path where the image will be saved
            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                imageFile.CopyTo(stream);
            }
            // return Ok("Image uploaded successfully");
            return new JsonResult(new { id = hash_code });
        }
        else
        {
            return BadRequest("Invalid image file");
        }
    }
}
