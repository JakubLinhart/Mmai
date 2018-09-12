using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mmai.Models
{
    public abstract class BaseRepository
    {
        protected CloudTable Table { get; }

        public BaseRepository(string tableName, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("tableStorage");
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            Table = tableClient.GetTableReference(tableName);
        }
    }
}
