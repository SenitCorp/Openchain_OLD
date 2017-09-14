using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenchainPlatform.Core.Data.Models
{
    public class Transaction
    {
        public Transaction()
        {
            Entries = new HashSet<TransactionEntry>();
        }

        /// <summary>
        /// Hash of the transaction
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The sequenceId (unique custom data provided when transaction is created)
        /// </summary>
        public string SequenceId { get; set; }

        /// <summary>
        /// UTC DateTime when the transaction was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Meta data (in the form of a json string) submitted with the transaction
        /// </summary>
        public string Metadata { get; set; }

        /// <summary>
        /// The entries associated with the transaction
        /// </summary>
        public virtual ICollection<TransactionEntry> Entries { get; set; }
    }
}
