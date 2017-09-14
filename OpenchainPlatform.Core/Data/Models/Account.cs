using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenchainPlatform.Core.Data.Models
{
    public class Account
    {
        /// <summary>
        /// The id of the account
        /// </summary>
        public string Id { get; set; }

        public string AssetId { get; set; }

        /// <summary>
        /// Encrypted seed
        /// </summary>
        public string Seed { get; set; }

        /// <summary>
        /// The path of the account on the ledger
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The alias of the account
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Balance of the account
        /// </summary>
        public long Balance { get; set; }

        /// <summary>
        /// Label of account
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// The chain of the account in the heirachy
        /// </summary>
        public string Chain { get; set; }

        public virtual Asset Asset { get; set; }

        public virtual ICollection<AccountAddress> Addresses { get; set; }
    }
}
