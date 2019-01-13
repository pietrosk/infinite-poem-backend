using FunctionApp1.ApiModels;
using FunctionApp1.TableModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FunctionApp1.Api.Verses
{
    public static class Add
    {
        [FunctionName("Add-Verse")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "verses/{variant}")] HttpRequest req,
            [Table("InfinitePoemV1")] CloudTable cloudTable,
            string variant,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<AddVerseModel>(requestBody);

            var item = new VerseEntity
            {
                CreatedAt = DateTime.UtcNow,
                PartitionKey = variant,
                RowKey = DateTime.UtcNow.ToString("o"),
                Text = data.Text,
            };

            var result = await cloudTable.ExecuteAsync(TableOperation.Insert(item));

            return (ActionResult)new OkObjectResult(Get.ToApiVerse(result.Result as VerseEntity));
        }
    }
}
