using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenchainPlatform.Core.Data.Models
{
    public class TransactionEntry
    {
        public int Id { get; set; }

        public string TransactionId { get; set; }
        public string LedgerPath { get; set; }
        public long Value { get; set; }

        public virtual Transaction Transaction { get; set; }
    }
}
