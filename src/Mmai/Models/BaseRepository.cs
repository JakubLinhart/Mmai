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

        protected async Task<IEnumerable<T>> ExecuteQueryAsync<T>(TableQuery<T> query) where T : ITableEntity, new()
        {
            var items = new List<T>();
            TableContinuationToken token = null;

            do
            {
                TableQuerySegment<T> seg = await Table.ExecuteQuerySegmentedAsync<T>(query, token);
                token = seg.ContinuationToken;
                items.AddRange(seg);

            } while (token != null);

            return items;
        }

    }
}
