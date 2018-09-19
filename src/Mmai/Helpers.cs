using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Mmai
{
    public static class Helpers
    {
        public static string NewGuid()
            => GuidToBase64(Guid.NewGuid());

        public static string GuidToBase64(Guid guid)
            => Convert.ToBase64String(guid.ToByteArray()).Replace("/", "-").Replace("+", "_").Replace("=", "");

        public static FileContentResult Csv<T>(this Controller controller, IEnumerable<T> elements, string name)
        {
            using (var stream = new MemoryStream(4096))
            {
                var writer = new StreamWriter(stream);
                var csvWriter = new CsvWriter(writer);

                csvWriter.WriteRecords(elements);
                csvWriter.Flush();
                writer.Flush();

                return controller.File(stream.GetBuffer(), "application/CSV", name);
            }
        }
    }
}
