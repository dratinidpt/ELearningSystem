using Microsoft.AspNetCore.Mvc;

namespace ELearningSystem.Controllers
{
    [Route("api/files")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public FilesController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        // Change route to accept full path with "uploads"
        [HttpGet("uploads/{folder}/{filename}")]
        public IActionResult GetFile(string folder, string filename)
        {
            try
            {
                var filePath = Path.Combine(_environment.WebRootPath, "uploads", folder, filename);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound(new { message = "File not found" });
                }

                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                return File(fileBytes, "application/pdf", filename);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}