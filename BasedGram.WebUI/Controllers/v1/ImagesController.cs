using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

using System.Drawing;

namespace BasedGram.WebUI.Controllers.v1;


[ApiController]
[ApiVersion(1)]
[Route("api/v{v:apiVersion}/[controller]")]
public class ImagesController : ControllerBase
{
    [HttpGet("{img_code:int}")]
    public IActionResult GetTestImage(int img_code)
    {
        var imagePath = $"/home/deadpizza/Рабочий стол/МГТУ/6 семестр/PPO/lab_09/BasedGram/BasedGram.WebUI/imgs/{img_code}.png"; // Replace with the actual path to test.png
        var image = System.IO.File.OpenRead(imagePath);
        return File(image, "image/png");
    }


    [HttpPost]
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
            return new JsonResult(new {id = hash_code});
        }
        else
        {
            return BadRequest("Invalid image file");
        }
    }
}
