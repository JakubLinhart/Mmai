using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mmai.Models
{
    public class Player : TableEntity
    {
        public string NickName { get; set; }
        public string Email { get; set; }
    }
}
