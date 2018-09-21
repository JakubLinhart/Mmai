using System.Threading.Tasks;

namespace Mmai.Models
{
    public interface IGameStatisticsRepository
    {
        Task<Leaderboard> GetTopTen(string currentPlayerId, string speciesName);
        Task<OneGameStatistics> GetOneGameStatistics(int currentTurnCount, string currentGameId, string currentPlayerId, string speciesName);
        Task<AllGamesStatistics> GetAllGamesStatistics(string speciesName);
    }
}
