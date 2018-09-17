using System.Threading.Tasks;

namespace Mmai.Models
{
    public interface ILeaderboardRepository
    {
        Task<Leaderboard> GetTopTen(string currentPlayerId, string speciesName);
    }
}
