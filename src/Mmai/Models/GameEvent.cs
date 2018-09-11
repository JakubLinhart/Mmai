using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mmai.Models
{
    public class GameEvent : TableEntity
    {
        public string GameId { get; set; }

        public string Label { get; set; }
        public string Card { get; set; }
        public DateTime Time { get; set; }
        public int? MillisecondsSinceLastEvent { get; set; }
        public string PlayerId { get; set; }
    }
}
