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
        private readonly IPlayerRepository playerRepository;

        public LeaderboardRepository(IConfiguration configuration, IGameRepository gameRepository,
            IPlayerRepository playerRepository)
        {
            this.gameRepository = gameRepository;
            this.playerRepository = playerRepository;
        }

        public async Task<Leaderboard> GetTopTen(string currentPlayerId, string speciesName)
        {
            var games = await gameRepository.GetAll();
            bool isDefaultSpecies = speciesName.Equals("littleowl", StringComparison.OrdinalIgnoreCase);
            var items = games
                .Where(x => (x.SpeciesName != null && x.SpeciesName.Equals(speciesName)) || (isDefaultSpecies && x.SpeciesName == null))
                .Where(x => x.MovesCount.HasValue)
                .GroupBy(x => x.PlayerId)
                .Select(x => new LeaderboardItem
                {
                    MovesCount = x.Min(y => y.MovesCount.Value),
                    PlayerId = x.First().PlayerId
                })
                .OrderBy(x => x.MovesCount)
                .Take(10)
                .ToArray();

            foreach (var item in items)
            {
                var player = await playerRepository.GetPlayer(item.PlayerId);
                item.NickName = player?.NickName
                    ?? (currentPlayerId != null && item.PlayerId.Equals(currentPlayerId, StringComparison.OrdinalIgnoreCase) 
                        ? "anonymous (you)" : "anonymous");
            }

            return new Leaderboard
            {
                Name = speciesName,
                Items = items.ToArray()
            };
        }
    }
}
