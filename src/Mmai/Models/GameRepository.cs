using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mmai.Models
{
    internal sealed class GameRepository : BaseRepository, IGameRepository
    {
        public GameRepository(IConfiguration configuration)
            : base("games", configuration)
        {
        }

        public Task<IEnumerable<Game>> GetAll() => ExecuteQueryAsync(new TableQuery<Game>());

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
