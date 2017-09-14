using System;
using System.Collections.Generic;
using System.Text;

namespace OpenchainPlatform.Core.Data.Models
{
    public class AccountAddress
    {
        public string Address { get; set; }
        public string AccountId { get; set; }
        public string Chain { get; set; }
        public uint Index { get; set; }

        public virtual Account Account { get; set; }
    }
}
