using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult Test(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest();
            var result = file.OpenReadStream();
           var str = _storageService.UploadAsync(result,"Hi this is Jadara");
            return Ok($"test {str}");
        }
    }
}
