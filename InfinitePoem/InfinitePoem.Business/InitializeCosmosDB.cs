using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using System.Threading.Tasks;

namespace InfinitePoem.Business
{
    public class VersesDBConfiguration
    {
        public string DatabaseName { get; set; }
        public string ContainerName { get; set; }
        public string ConnectionString { get; set; }
    }

    public static class CosmosDBInitializer
    {
        public static async Task<CosmosDBService> InitializeCosmosClientInstanceAsync(VersesDBConfiguration dbConfig)
        {
            string databaseName = dbConfig.DatabaseName;
            string containerName = dbConfig.ContainerName;
            var connectionString = dbConfig.ConnectionString;
            CosmosClientBuilder clientBuilder = new CosmosClientBuilder(connectionString);
            CosmosClient client = clientBuilder
                                .WithConnectionModeDirect()
                                .Build();
            CosmosDBService cosmosDbService = new CosmosDBService(client, databaseName, containerName);
            DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
            await database.Database.CreateContainerIfNotExistsAsync(containerName, "/_pk");

            return cosmosDbService;
        }
    }
}
