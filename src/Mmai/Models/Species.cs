using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mmai.Models
{
    public class Species
    {
        public string Name { get; set; }
        public int CardCount { get; set; }
        public SpeciesVoiceSet[] Sets { get; set; }
    }

    public class SpeciesVoiceSet
    {
        public string Name { get; set; }
        public string[][] SubSets { get; set; }
    }
}
