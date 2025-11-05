using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IStorageService _storageService;
        public AccountsController(IStorageService storageService) {
            _storageService = storageService;
        }

        [HttpPost]
        public async Task<IActionResult> Test(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { Message = "File is empty" });

            // Generate GUID filename while preserving extension
            var originalFileName = file.FileName;
            var fileExtension = Path.GetExtension(originalFileName);
            var guidFileName = $"{Guid.NewGuid()}{fileExtension}";

            using var fileStream = file.OpenReadStream();
            

            var fullUrl = await _storageService.UploadAsync(fileStream, guidFileName);

            if (string.IsNullOrEmpty(fullUrl))
                return StatusCode(500, new { Message = "File upload failed" });

            return Ok(new
            {
                URL = fullUrl
            });
        }
    }
}
