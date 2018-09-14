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
            gameEvent.RowKey = gameEvent.Time.Ticks.ToString() + gameEvent.GameId;

            await Table.ExecuteAsync(TableOperation.Insert(gameEvent));
            return gameEvent;
        }

        public Task<IEnumerable<GameEvent>> GetAll()
            => ExecuteQueryAsync(new TableQuery<GameEvent>());
    }
}
