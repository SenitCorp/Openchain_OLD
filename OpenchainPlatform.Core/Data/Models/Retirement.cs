using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenchainPlatform.Core.Data.Models
{
    public class Retirement
    {
        public string Id { get; set; }
        public string TransactionId { get; set; }
        public string AssetId { get; set; }
        public long value { get; set; }
    }
}
