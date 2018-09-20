using System.Threading.Tasks;

namespace Mmai.Models
{
    public interface ISpeciesRepository
    {
        Task<Species> Get(string name);
    }
}