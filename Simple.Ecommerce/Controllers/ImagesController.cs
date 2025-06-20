using ImageFile.Library.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Simple.Ecommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImagesController : ControllerBase
    {
        private readonly IImageManager _imageManager;

        public ImagesController(
            IImageManager imageManager
        )
        {
            _imageManager = imageManager;
        }

        [HttpGet("{fileName}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetImage(string fileName)
        {
            try
            {
                var stream = await _imageManager.GetImageAsync(fileName);

                var contentType = GetContentType(fileName);

                return File(stream, contentType, fileName);
            }
            catch
            {
                return NotFound();
            }
        }

        private string GetContentType(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();

            return ext switch
            {
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                _ => "application/octet-stream",
            };
        }
    }
}
