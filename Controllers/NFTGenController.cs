using Microsoft.AspNetCore.Mvc;
using NFTGenApi.Models;
using NFTGenApi.Services;

namespace NFTGenApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NFTGenController : ControllerBase
    {

        private readonly ILogger<NFTGenController> _logger;

        public NFTGenController(ILogger<NFTGenController> logger)
        {
            _logger = logger;
        }

        [HttpGet("GetBlank")]
        public ActionResult<Properties> Get() => Ok(GenerateJson.GenerateDummyProperties());
    }
}