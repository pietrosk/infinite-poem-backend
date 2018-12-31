using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;

namespace FunctionApp1
{
    public static class verse
    {
        [FunctionName("verse")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string language = req.Query["language"];
            var validLanguage = new string[] { "en", "sk" }.Any(x => x == language);

            if (!validLanguage)
                return new BadRequestObjectResult("Please pass a valid language");

            var allVerses = new
            {
                en = new string[] { "first", "second line", "another line below" },
                sk = new string[] { "prvy riadok", "druhy riadok", "treti riadok" },
            };

            var verses = language.Equals("en", StringComparison.OrdinalIgnoreCase)
                ? allVerses.en
                : allVerses.sk;

            return new OkObjectResult(verses);
        }
    }
}
