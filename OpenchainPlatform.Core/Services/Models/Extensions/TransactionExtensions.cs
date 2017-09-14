using OpenchainPlatform.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenchainPlatform.Core.Services.Models.Extensions
{
    public static class TransactionExtensions
    {
        public static Transaction ToEntity(this TransactionModel source)
        {
            if(source == null)
            {
                return null;
            }

            var destination = new Transaction
            {
                Id = source.Id,
                CreatedAt = DateTime.UtcNow,
                Metadata = string.Empty,
                SequenceId = string.Empty
            };

            foreach(var item in source.Entries)
            {
                destination.Entries.Add(new TransactionEntry
                {
                    LedgerPath = item.Path,
                    Value = item.Value
                });
            }

            return destination;
        }
    }
}
