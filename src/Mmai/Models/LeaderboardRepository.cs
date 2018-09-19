using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System;
using Microsoft.WindowsAzure.Storage.Table;
using System.Linq;

namespace Mmai.Models
{
    internal sealed class LeaderboardRepository : ILeaderboardRepository
    {
        private readonly IGameRepository gameRepository;

        public LeaderboardRepository(IConfiguration configuration, IGameRepository gameRepository)
        {
            this.gameRepository = gameRepository;
        }

        public async Task<Leaderboard> GetTopTen(string currentPlayerId, string speciesName)
        {
            var games = await gameRepository.GetAll();
            bool isDefaultSpecies = speciesName.Equals("littleowl", StringComparison.OrdinalIgnoreCase);
            var items = games
                .Where(x => (x.SpeciesName != null && x.SpeciesName.Equals(speciesName)) || (isDefaultSpecies && x.SpeciesName == null))
                .Where(x => x.MovesCount.HasValue)
                .OrderBy(x => x.MovesCount)
                .Take(10)
                .Select(x => new LeaderboardItem
                {
                    MovesCount = x.MovesCount.Value,
                    PlayerId = x.PlayerId,
                    NickName = x.NickName ?? "anonymous"
                })
                .ToArray();

            return new Leaderboard
            {
                Name = speciesName,
                Items = items.ToArray()
            };
        }
    }
}
