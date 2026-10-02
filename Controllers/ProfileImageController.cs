using Microsoft.AspNetCore.Mvc;
using Niwahana_backend.Business;
using Niwahana_backend.Models.Domain;
using Microsoft.AspNetCore.Authorization;


namespace Niwahana_backend.Controllers
{

    [ApiController]
    [Route("api/profile-image")]
    [Authorize]
    public class ProfileImageController : ControllerBase
    {
        private readonly ProfileImageBusiness _business;

        public ProfileImageController(ProfileImageBusiness business)
        {
            _business = business;
        }

        //Get image
        [HttpGet("getimage")]
        public IActionResult GetImage(int userId)
        {
            try
            {
                var image =  _business.GetImage(userId);

                if (image == null ||image.ImageData == null ||image.ImageData.Length == 0)
                {
                    return NotFound(new
                    {
                        message = "Profile image not found."
                    });
                }

                return File(image.ImageData,image.ContentType?? "image/jpeg");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get image error: {ex}");

                return StatusCode(
                    500,
                    new
                    {
                        message = "Failed to load profile image."
                    });
            }
        }

        // ADD / UPDATE Image
       
        [HttpPost("insertimage")]
        public IActionResult UploadImage(int userId,IFormFile file)
        {
            try
            {
                if (file == null ||file.Length == 0)
                {
                    return BadRequest(new
                    {
                        message = "No image file was received."
                    });
                }

                var allowedTypes = new[]
                    {
                        "image/jpeg",
                        "image/jpg",
                        "image/png",
                        "image/webp"
                    };

                if (!allowedTypes.Contains(file.ContentType.ToLower()))
                {
                    return BadRequest(new
                    {
                        message = "Only JPG, PNG and WEBP images are allowed."
                    });
                }

                // 5 MB limit
                const long maxFileSize = 5 * 1024 * 1024;

                if (file.Length > maxFileSize)
                {
                    return BadRequest(new
                    {
                        message ="Image size must be less than 5 MB."
                    });
                }

                using var memoryStream = new MemoryStream();
                file.CopyTo(memoryStream);
                var image = new ProfileImageMl
                    {
                        UserId = userId,
                        ImageData = memoryStream.ToArray(),
                        ContentType = file.ContentType
                    };

                var saved =  _business.SaveImage(image);

                if (!saved)
                {
                    return StatusCode(500,
                        new
                        {
                            message = "Image could not be saved."
                        });
                }

                return Ok(new
                {
                    message = "Profile image uploaded successfully."
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Upload image error: {ex}");

                return StatusCode(
                    500,
                    new
                    {
                        message = "Failed to upload profile image."
                    });
            }
        }


        // DELETE Image

        [HttpDelete("deleteimage")]
        public IActionResult DeleteImage(int userId)
        {
            try
            {
                var deleted =  _business.DeleteImage(userId);

                if (!deleted)
                {
                    return NotFound(new
                    {
                        message = "Profile image not found."
                    });
                }

                return Ok(new
                {
                    message = "Profile image deleted successfully."
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Delete image error: {ex}");

                return StatusCode(
                    500,
                    new
                    {
                        message = "Failed to delete profile image."
                    });
            }
        }
    }
}
