using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Mmai.Data
{
    public class Repository
    {
        private static Lazy<CloudStorageAccount> storageAccount = new Lazy<CloudStorageAccount>(() =>
            CloudStorageAccount.Parse("UseDevelopmentStorage=true;"), true);

        private static Lazy<CloudTable> gameEventsTable = new Lazy<CloudTable>(() =>
        {
            var tableClient = storageAccount.Value.CreateCloudTableClient();
            var table = tableClient.GetTableReference("gameEvents");
            return table;
        }, true);

        public static async Task<GameEvent> AddGameEvent(string playerId, string gameId, string label, string card, DateTime time, int? millisecondsSinceLastEvent)
        {
            await gameEventsTable.Value.CreateIfNotExistsAsync();

            var gameEvent = new GameEvent(playerId, gameId, label, card, time, millisecondsSinceLastEvent);
            await gameEventsTable.Value.ExecuteAsync(TableOperation.Insert(gameEvent));
            return gameEvent;
        }

        public static string NewGuid() 
            => GuidToBase64(Guid.NewGuid());

        private static string GuidToBase64(Guid guid) 
            => Convert.ToBase64String(guid.ToByteArray()).Replace("/", "-").Replace("+", "_").Replace("=", "");
    }
}
