using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mmai.Models
{
    public interface IGameEventRepository
    {
        Task<GameEvent> Insert(GameEvent gameEvent);
        Task<IEnumerable<GameEvent>> GetAll();
    }
}
