using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mmai.Models
{
    public interface IPlayerRepository
    {
        Task<Player> GetPlayer(string id);
        Task<IEnumerable<Player>> GetAll();
        Task<Player> Update(string playerId, string nickName, string email);
    }
}
