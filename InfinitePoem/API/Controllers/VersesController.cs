using API.Models;
using InfinitePoem.Business.Api;
using InfinitePoem.DAO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VersesController : ControllerBase
    {
        public VersesController(ICosmosDBService cosmosDBService)
        {
            _cosmosDbService = cosmosDBService;
        }

        private readonly ICosmosDBService _cosmosDbService;

        [HttpGet("{language}")]
        public async Task<ActionResult<IList<ApiVerseResult>>> GetAll([FromRoute] string language)
        {
            // var result = new List<ApiVerseResult> { new ApiVerseResult { Text = "test" } };

            var verses = await _cosmosDbService.GetItemsAsync(language);

            return new OkObjectResult(verses.Select(MapToApi));
        }

        [HttpPost]
        public ActionResult Create()
        {
            return new OkResult();
        }

        [HttpPost("{language}")]
        public async Task<ActionResult<ApiVerseResult>> Create([FromBody] ApiVerseCreateRequest request, [FromRoute] string language)
        {
            var result = await _cosmosDbService.AddItemAsync(new InfinitePoem.DAO.Verse
            {
                Id = Guid.NewGuid().ToString(),
                Text = request.Text
            }, language);

            return new OkObjectResult(MapToApi(result));
        }

        private ApiVerseResult MapToApi(Verse verse)
        {
            return new ApiVerseResult
            {
                Id = Guid.Parse(verse.Id),
                Text = verse.Text,
            };
        }
    }
}