﻿using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mmai.Models
{
    public class Game : TableEntity
    {
        public string PlayerId { get; set; }
        public string SpeciesId { get; set; }
        public string Id { get; set; }
        public DateTime? FinishedTime { get; set; }
        public int? Duration { get; set; }
        public int? MovesCount { get; set; }
        public int? TurnsCount { get; set; }
        public string Email { get; set; }
        public string NickName { get; set; }
    }
}
