using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mmai.Models
{
    internal sealed class PlayerRepository : BaseRepository, IPlayerRepository
    {
        public PlayerRepository(IConfiguration configuration)
            : base("players", configuration)
        {
        }

        public Task<IEnumerable<Player>> GetAll()
        {
            return ExecuteQueryAsync(new TableQuery<Player>());
        }

        public async Task<Player> GetPlayer(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            string queryString = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, id);
            TableQuery<Player> query = new TableQuery<Player>().Where(queryString);

            IEnumerable<Player> players = await ExecuteQueryAsync(query);

            return players.FirstOrDefault();
        }

        public async Task<Player> Update(string playerId, string nickName, string email)
        {
            await Table.CreateIfNotExistsAsync();

            var player = new Player()
            {
                RowKey = playerId,
                PartitionKey = playerId,
                NickName = nickName,
                Email = email,
            };

            await Table.ExecuteAsync(TableOperation.InsertOrReplace(player));
            return player;
        }
    }
}
