﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mmai.Models
{
    public class Species
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CardCount { get; set; }
        public int ColumnCount { get; set; }
        public SpeciesVoiceSet[] Sets { get; set; }
    }
}
