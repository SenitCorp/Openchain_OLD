using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenchainPlatform.Core.Services.Models
{
    public class TransactionModel
    {
        public TransactionModel()
        {
            Entries = new List<TransactionEntryModel>();
        }

        public string Id { get; set; }

        public List<TransactionEntryModel> Entries { get; set; }
    }
}
