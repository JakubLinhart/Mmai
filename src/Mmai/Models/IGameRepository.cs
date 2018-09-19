using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mmai.Models
{
    public interface IGameRepository
    {
        Task Update(Game game);
        Task<Game> Insert(Game game);
        Task<IEnumerable<Game>> GetAll();
    }
}
