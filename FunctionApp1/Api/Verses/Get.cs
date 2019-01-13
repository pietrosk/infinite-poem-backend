using FunctionApp1.ApiModels;
using FunctionApp1.TableModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System.Linq;
using System.Threading.Tasks;

namespace FunctionApp1.Api.Verses
{
    public static class Get
    {
        [FunctionName("Get-Verse")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "verses/{variant}")] HttpRequest req,
            [Table("InfinitePoemV1")] CloudTable cloudTable,
            string variant,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var validVariant = new string[] { "en", "sk" }.Any(x => x == variant);

            if (!validVariant)
                return new BadRequestObjectResult("Please pass a valid language");

            var query = new TableQuery<VerseEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, variant));
            var token = new TableContinuationToken();
            var verses = (await cloudTable.ExecuteQuerySegmentedAsync(query, token)).ToList();

            var apiVerses = verses.Select(ToApiVerse);

            return new OkObjectResult(verses);
        }

        public static VerseModel ToApiVerse(VerseEntity item)
        {
            return new VerseModel
            {
                Text = item.Text,
            };
        }
    }
}
