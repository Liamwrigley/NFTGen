using Microsoft.AspNetCore.Mvc;
using NFTGenApi.Models;
using NFTGenApi.Services.Dummy;
using NFTGenApi.Services.MatrixGenerator;

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

        /// <summary>
        /// Returns the dummy shape of the properties object.
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetBlank")]
        public ActionResult<Properties> Get() => Ok(GenerateJson.GenerateDummyProperties());

        /// <summary>
        /// Takes a properties object and turns it into an adjacency matrix with index mapping
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        [HttpPost("Matrix")]
        public ActionResult<MatrixModel> Gen([FromBody] Properties properties) => Ok(MatrixGenerator.CreateMatrix(properties).ToModel());
    }
}