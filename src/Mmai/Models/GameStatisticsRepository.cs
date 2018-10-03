using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Mmai.Models
{
    internal sealed class GameStatisticsRepository : IGameStatisticsRepository
    {
        private readonly IGameRepository gameRepository;
        private readonly ISpeciesRepository speciesRepository;

        public GameStatisticsRepository(IConfiguration configuration, IGameRepository gameRepository,
            ISpeciesRepository speciesRepository)
        {
            this.gameRepository = gameRepository;
            this.speciesRepository = speciesRepository;
        }

        public async Task<AllGamesStatistics> GetAllGamesStatistics(string speciesId)
        {
            var games = await gameRepository.GetAll();
            var relevantGames = games.Where(x => (x.MovesCount.HasValue || x.TurnsCount.HasValue) && x.SpeciesId.Equals(speciesId, StringComparison.OrdinalIgnoreCase));

            var count = relevantGames.Count();

            if (count == 0)
            {
                return new AllGamesStatistics
                {
                    SpeciesId = speciesId,
                    MeanTurnsCount = "--"
                };
            }

            double sum = relevantGames.Sum(x => x.TurnsCount ?? x.MovesCount.Value / 2);
            return new AllGamesStatistics
            {
                SpeciesId = speciesId,
                MeanTurnsCount = Math.Round(sum / count, 2).ToString("#.#")
            };
        }

        public async Task<OneGameStatistics> GetOneGameStatistics(int currentTurnCount, string currentGameId, string currentPlayerId, string speciesId)
        {
            var games = await gameRepository.GetAll();

            var relevantGames = games
                .Where(x => (x.MovesCount.HasValue || x.TurnsCount.HasValue) && x.SpeciesId.Equals(speciesId, StringComparison.OrdinalIgnoreCase));

            var relevantGamesCount = relevantGames.Count();

            if (relevantGamesCount == 0)
            {
                return new OneGameStatistics()
                {
                    SpeciesId = speciesId,
                    BestTurnsCount = "--",
                    BetterThanPercentage = "--"
                };
            }

            var betterThanCount = relevantGames
                .Count(x => (x.TurnsCount ?? x.MovesCount / 2) > currentTurnCount);
            var bestTurnCount = relevantGames.Any()
                ? relevantGames.Min(x => x.TurnsCount ?? x.MovesCount.Value / 2).ToString() : "--";
            var betterPercentage = (int)(100 * ((double)betterThanCount / relevantGamesCount));

            return new OneGameStatistics()
            {
                SpeciesId = speciesId,
                BestTurnsCount = bestTurnCount,
                BetterThanPercentage = betterPercentage.ToString()
            };
        }

        public async Task<Leaderboard> GetTopTen(string currentPlayerId, string speciesId)
        {
            var games = await gameRepository.GetAll();
            var isDefaultSpecies = speciesId.Equals("littleowl", StringComparison.OrdinalIgnoreCase);
            var items = games
                .Where(x => (x.SpeciesId != null && x.SpeciesId.Equals(speciesId)) || (isDefaultSpecies && x.SpeciesId == null))
                .Where(x => x.TurnsCount.HasValue || x.MovesCount.HasValue)
                .OrderBy(x => x.TurnsCount ?? x.MovesCount)
                .Take(10)
                .Select(x => new LeaderboardItem
                {
                    MovesCount = x.TurnsCount ?? x.MovesCount.Value,
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
