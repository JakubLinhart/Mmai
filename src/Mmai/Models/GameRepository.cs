using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

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

            DateTime gameTime = game.FinishedTime ?? DateTime.UtcNow;
            game.Id = game.Id ?? gameTime.Ticks.ToString() + Helpers.NewGuid();

            game.PartitionKey = game.PlayerId;
            game.RowKey = game.Id;

            await Table.ExecuteAsync(TableOperation.Insert(game));
            return game;
        }

        public async Task Update(Game game)
        {
            await Table.CreateIfNotExistsAsync();

            game.PartitionKey = game.PlayerId;
            game.RowKey = game.Id;

            await Table.ExecuteAsync(TableOperation.InsertOrReplace(game));
        }

        public async Task<Game> UpdateContact(string playerId, string id, string nickName, string email)
        {
            var result = await Table.ExecuteAsync(TableOperation.Retrieve<Game>(playerId, id));
            var game = (Game)result.Result;

            game.NickName = nickName;
            game.Email = email;

            await Update(game);

            return game;
        }
    }
}
