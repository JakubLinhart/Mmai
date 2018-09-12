using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Table;

namespace Mmai.Models
{
    internal sealed class GameRepository : BaseRepository, IGameRepository
    {
        public GameRepository(IConfiguration configuration)
            : base("games", configuration)
        {
        }

        public async Task<Game> Insert(Game game)
        {
            await Table.CreateIfNotExistsAsync();

            game.PartitionKey = game.PlayerId;
            game.RowKey = game.Id;

            await Table.ExecuteAsync(TableOperation.Insert(game));
            return game;
        }
    }
}
