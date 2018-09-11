using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Mmai.Models
{
    public class GameEventRepository : IGameEventRepository
    {
        private readonly string connectionString;
        private CloudStorageAccount storageAccount;
        private CloudTable gameEventsTable;

        private CloudTable CreateTableClient(string tableName)
        {
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("gameEvents");
            return table;
        }

        public GameEventRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("tableStorage");
            storageAccount = CloudStorageAccount.Parse(connectionString);
            gameEventsTable = CreateTableClient("gameEvents");
        }

        public async Task<GameEvent> Insert(GameEvent gameEvent)
        {
            await gameEventsTable.CreateIfNotExistsAsync();

            gameEvent.PartitionKey = gameEvent.PlayerId;
            gameEvent.RowKey = NewGuid();

            await gameEventsTable.ExecuteAsync(TableOperation.Insert(gameEvent));
            return gameEvent;
        }

        public Task<IEnumerable<GameEvent>> GetAll()
            => ExecuteQueryAsync(gameEventsTable, new TableQuery<GameEvent>());

        internal static async Task<IEnumerable<T>> ExecuteQueryAsync<T>(CloudTable table, TableQuery<T> query) where T : ITableEntity, new()
        {
            var items = new List<T>();
            TableContinuationToken token = null;

            do
            {
                TableQuerySegment<T> seg = await table.ExecuteQuerySegmentedAsync<T>(query, token);
                token = seg.ContinuationToken;
                items.AddRange(seg);

            } while (token != null);

            return items;
        }


        public string NewGuid()
            => GuidToBase64(Guid.NewGuid());

        private static string GuidToBase64(Guid guid)
            => Convert.ToBase64String(guid.ToByteArray()).Replace("/", "-").Replace("+", "_").Replace("=", "");

    }
}
