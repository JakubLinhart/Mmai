using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mmai.Data
{
    public class Game : TableEntity
    {
        public Game() { }

        public Game(string id, string playerId)
        {
            this.PartitionKey = playerId;
            this.RowKey = id;
        }

        public string Id => this.RowKey;
        public string PlayerId => this.PartitionKey;
    }
}
