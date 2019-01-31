using FunctionApp1.ApiModels;
using FunctionApp1.TableModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FunctionApp1.Api.Verses
{
    public static class Get
    {
        [FunctionName("Get-Verses")]
        public static async Task<HttpResponseMessage> GetVerses(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "verses/{variant}")] HttpRequestMessage request,
            [Table("InfinitePoemV1")] CloudTable cloudTable,
            string variant,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            if (!ValidateLanguageVariant(variant))
                return request.CreateErrorResponse(System.Net.HttpStatusCode.BadRequest, "Please pass a valid language");

            var query = new TableQuery<VerseEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, variant))
                .Take(2);
            var token = new TableContinuationToken();
            var queryResult = await cloudTable.ExecuteQuerySegmentedAsync(query, null);
            var apiVerses = queryResult.Select(ToApiVerse).ToList();

            var response = request.CreateResponse(System.Net.HttpStatusCode.OK, apiVerses);
            response.Headers.Add("ContinuationToken", Newtonsoft.Json.JsonConvert.SerializeObject(queryResult.ContinuationToken));

            return response;
        }

        [FunctionName("Get-Verses-Next")]
        public static async Task<HttpResponseMessage> GetVersesNext(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "verses/{variant}/next")] HttpRequestMessage request,
            [Table("InfinitePoemV1")] CloudTable cloudTable,
            string variant,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            if (!ValidateLanguageVariant(variant))
                return request.CreateErrorResponse(System.Net.HttpStatusCode.BadRequest, "Please pass a valid language");

            var stringContent = await request.Content.ReadAsStringAsync();
            var tableContinuationToken = JsonConvert.DeserializeObject<TableContinuationToken>(stringContent);

            var query = new TableQuery<VerseEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, variant))
                .Take(2);
            var queryResult = await cloudTable.ExecuteQuerySegmentedAsync(query, tableContinuationToken);
            var apiVerses = queryResult.Select(ToApiVerse).ToList();

            var response = request.CreateResponse(System.Net.HttpStatusCode.OK, apiVerses);
            response.Headers.Add("ContinuationToken", JsonConvert.SerializeObject(queryResult.ContinuationToken));

            return response;
        }

        public static VerseModel ToApiVerse(VerseEntity item)
        {
            return new VerseModel
            {
                Text = item.Text,
            };
        }

        private static bool ValidateLanguageVariant(string variant)
        {
            return new string[] { "en", "sk" }.Any(x => x == variant);
        }
    }
}
