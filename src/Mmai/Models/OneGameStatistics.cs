using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mmai.Models
{
    public sealed class OneGameStatistics
    {
        public string SpeciesId { get; set; }

        public int BestTurnsCount { get; set; }
        public int BetterThanPercentage { get; set; }
    }
}
