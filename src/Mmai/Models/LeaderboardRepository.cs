using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System;
using Microsoft.WindowsAzure.Storage.Table;
using System.Linq;

namespace Mmai.Models
{
    internal sealed class LeaderboardRepository : BaseRepository, ILeaderboardRepository
    {
        public LeaderboardRepository(IConfiguration configuration) : base("games", configuration)
        {
        }

        public async Task<Leaderboard> GetTopTen(string speciesName)
        {
            var games = await ExecuteQueryAsync(new TableQuery<Game>());
            bool isDefaultSpecies = speciesName.Equals("sycek", StringComparison.OrdinalIgnoreCase);
            var items = games
                .Where(x => (x.SpeciesName != null && x.SpeciesName.Equals(speciesName)) || (isDefaultSpecies && x.SpeciesName == null))
                .GroupBy(x => x.PlayerId)
                .Select(x => new LeaderboardItem
                {
                    MovesCount = x.Max(y => y.MovesCount),
                    Name = x.First().PlayerId
                })
                .OrderBy(x => x.MovesCount)
                .Take(10);

            return new Leaderboard
            {
                Name = speciesName,
                Items = items.ToArray()
            };
        }
    }
}
