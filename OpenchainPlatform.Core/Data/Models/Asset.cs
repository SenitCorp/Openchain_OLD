using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenchainPlatform.Core.Data.Models
{
    public class Asset
    {
        public Asset()
        {
            Accounts = new HashSet<Account>();
        }

        public string Id { get; set; }

        public string AccountAddress { get; set; }

        public string Path { get; set; }

        public string Label { get; set; }

        public string Chain { get; set; }

        public long IssuedUnits { get; set; }

        public int Index { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }
    }
}
