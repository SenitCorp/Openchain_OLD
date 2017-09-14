using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenchainPlatform.Core.Services.Models
{
    public class AccountCredentialsModel
    {
        public string Seed { get; set; }
        public string Chain { get; set; }
        public uint Index { get; set; }
        public string Passphrase { get; set; }
    }
}
