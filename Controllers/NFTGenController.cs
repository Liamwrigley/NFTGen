using Microsoft.AspNetCore.Mvc;
using NFTGenApi.Models;
using NFTGenApi.Services.Dummy;
using NFTGenApi.Services.Generator;

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
        public ActionResult<MatrixModel> Matrix([FromBody] Properties properties) => Ok(MatrixGenerator.CreateMatrix(properties).ToModel());

        /// <summary>
        /// Takes a properties object and turns it into nfts
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        [HttpPost("Generate")]
        public ActionResult<Buckets> Generate([FromBody] Properties properties) => Ok(Engine.Generate(properties));
    }
}