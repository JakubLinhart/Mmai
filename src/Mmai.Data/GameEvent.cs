using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mmai.Data
{
    public class GameEvent : TableEntity
    {
        public GameEvent() { }

        public GameEvent(string playerId, string gameId, string label, string card, DateTime time, int? millisecondsSinceLastEvent)
        {
            RowKey = Repository.NewGuid();
            PartitionKey = playerId;
            Label = label;
            Time = time;
            Card = card;
            GameId = gameId;
            MillisecondsSinceLastEvent = millisecondsSinceLastEvent;
        }

        public string Id => RowKey;
        public string PlayerId => PartitionKey;

        public string GameId { get; set; }
        public string Label { get; set; }
        public string Card { get; set; }
        public DateTime Time { get; set; }
        public int? MillisecondsSinceLastEvent { get; set; }
    }
}
