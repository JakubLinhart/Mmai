using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mmai.Models
{
    public interface IGameRepository
    {
        Task<Game> Insert(Game game);
    }
}
