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
        private readonly ISpeciesRepository speciesRepository;

        public LeaderboardRepository(IConfiguration configuration, IGameRepository gameRepository,
            ISpeciesRepository speciesRepository)
        {
            this.gameRepository = gameRepository;
            this.speciesRepository = speciesRepository;
        }

        public async Task<Leaderboard> GetTopTen(string currentPlayerId, string speciesId)
        {
            var games = await gameRepository.GetAll();
            bool isDefaultSpecies = speciesId.Equals("littleowl", StringComparison.OrdinalIgnoreCase);
            var items = games
                .Where(x => (x.SpeciesId != null && x.SpeciesId.Equals(speciesId)) || (isDefaultSpecies && x.SpeciesId == null))
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

            var species = await speciesRepository.Get(speciesId);

            return new Leaderboard
            {
                Name = species.Name,
                Items = items.ToArray()
            };
        }
    }
}
