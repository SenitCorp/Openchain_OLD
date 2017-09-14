using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenchainPlatform.Core.Services.Models
{
    public class AccountModel
    {
        public string Seed { get; set; }

        public string Path { get; set; }

        public string Base58Address { get; set; }

        public long Balance { get; set; }

        public string Chain { get; set; }
    }
}
