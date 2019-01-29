using FunctionApp1.ApiModels;
using FunctionApp1.TableModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FunctionApp1.Api.Verses
{
    public static class Get
    {
        [FunctionName("Get-Verse")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "verses/{variant}")] HttpRequestMessage request,
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
            var queryResult = await cloudTable.ExecuteQuerySegmentedAsync(query, null);

            var apiVerses = queryResult.ToList().Select(ToApiVerse);

            // add continuation token as a header using action filter https://stackoverflow.com/questions/32017686/add-a-custom-response-header-in-apicontroller/52264433
            // queryResult.ContinuationToken

            return new OkObjectResult(apiVerses);
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
