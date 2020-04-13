using InfinitePoem.Business.Api;
using InfinitePoem.DAO;
using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfinitePoem.Business
{
    public class CosmosDBService : ICosmosDBService
    {
        private const string VersesPartitionKeyPrefix = "V;";
        private Container _container;

        public CosmosDBService(
            CosmosClient dbClient,
            string databaseName,
            string containerName)
        {
            this._container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task<Verse> AddItemAsync(Verse item, string language)
        {
            item.PartitionKey = GetPartitionKey(language).ToString();
            var result = await this._container.CreateItemAsync<Verse>(item, new PartitionKey(item.PartitionKey));
            return result.Resource;
        }

        public async Task DeleteItemAsync(string id)
        {
            await this._container.DeleteItemAsync<Verse>(id, new PartitionKey(id));
        }

        public async Task<Verse> GetItemAsync(string id)
        {
            try
            {
                ItemResponse<Verse> response = await this._container.ReadItemAsync<Verse>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<IEnumerable<Verse>> GetItemsAsync(string language)
        {
            var queryDefinition = new QueryDefinition("select * from c");
            var query = this._container.GetItemQueryIterator<Verse>(queryDefinition, null,
                new QueryRequestOptions
                {
                    PartitionKey = new PartitionKey(GetPartitionKey(language)),
                });

            var results = new List<Verse>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                results.AddRange(response.ToList());
            }

            return results;
        }

        private static string GetPartitionKey(string language)
        {
            return $"{VersesPartitionKeyPrefix}{language}";
        }

        public async Task UpdateItemAsync(string id, Verse item)
        {
            await this._container.UpsertItemAsync<Verse>(item, new PartitionKey(id));
        }
    }
}



