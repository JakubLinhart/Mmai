using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mmai.Models
{
    public sealed class OneGameStatistics
    {
        public string SpeciesId { get; set; }

        public string BestTurnsCount { get; set; }
        public string BetterThanPercentage { get; set; }
    }
}
