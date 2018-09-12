using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Mmai.Models
{
    internal sealed class GameEventRepository : BaseRepository, IGameEventRepository
    {
        public GameEventRepository(IConfiguration configuration)
            : base("gameEvents", configuration)
        {
        }

        public async Task<GameEvent> Insert(GameEvent gameEvent)
        {
            await Table.CreateIfNotExistsAsync();

            gameEvent.PartitionKey = gameEvent.PlayerId;
            gameEvent.RowKey = NewGuid();

            await Table.ExecuteAsync(TableOperation.Insert(gameEvent));
            return gameEvent;
        }

        public Task<IEnumerable<GameEvent>> GetAll()
            => ExecuteQueryAsync(Table, new TableQuery<GameEvent>());

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
