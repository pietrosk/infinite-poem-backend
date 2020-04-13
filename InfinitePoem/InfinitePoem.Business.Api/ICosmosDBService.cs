using InfinitePoem.DAO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InfinitePoem.Business.Api
{
    public interface ICosmosDBService
    {
        Task<IEnumerable<Verse>> GetItemsAsync(string language);
        Task<Verse> GetItemAsync(string id);
        Task<Verse> AddItemAsync(Verse item, string language);
        Task UpdateItemAsync(string id, Verse item);
        Task DeleteItemAsync(string id);
    }
}
