using InfinitePoem.Business;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Seeder
{
    class Program
    {
        public static IConfigurationRoot configuration;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello Seeder! Waiting for initializing DB...");

            var versesDBConfiguration = GetCosmosDBConfiguration();

            var cosmosDBService = CosmosDBInitializer.InitializeCosmosClientInstanceAsync(versesDBConfiguration).GetAwaiter().GetResult();

            Console.WriteLine("Ready! Press enter to start seeding!");
            Console.ReadLine();

            var poem = @"
Forever we remain oblivious to the future,
lost to the past and enduring our torture.
Forever we take chances to settle our scores,
losing some battles and winning some wars.
Forever praying out loud hoping someone will hear,
forever crying softly but never shedding a tear.
Forever exists behind a disguise,
but the belief in forever keeps our hearts alive.
".Split(',', '.').ToList();

            poem.ForEach(async line =>
            {
                Console.WriteLine($"Adding line '{line}'");
                await cosmosDBService.AddItemAsync(new InfinitePoem.DAO.Verse
                {
                    CreatedAt = DateTime.UtcNow.ToString(),
                    Id = Guid.NewGuid().ToString(),
                    Text = line,
                }, "en");
            });

            Console.WriteLine("Poem added!");
            Console.ReadLine();
        }

        private static VersesDBConfiguration GetCosmosDBConfiguration()
        {
            var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json");

            var configuration = builder.Build();

            return new VersesDBConfiguration
            {
                ConnectionString = configuration.GetSection("CosmosDB").GetSection("ConnectionString").Value,
                DatabaseName = configuration.GetSection("CosmosDB").GetSection("DatabaseName").Value,
                ContainerName = configuration.GetSection("CosmosDB").GetSection("ContainerName").Value,
            };
        }
    }
}
