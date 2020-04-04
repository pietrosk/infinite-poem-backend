using API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VersesController : ControllerBase
    {
        [HttpGet("{language}")]
        public ActionResult<IList<ApiVerseResult>> GetAll(string language)
        {
            var result = new List<ApiVerseResult> { new ApiVerseResult { Text = "test" } };

            return new OkObjectResult(result);
        }
    }
}