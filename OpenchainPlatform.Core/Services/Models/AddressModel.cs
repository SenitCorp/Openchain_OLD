using System;
using System.Collections.Generic;
using System.Text;

namespace OpenchainPlatform.Core.Services.Models
{
    public class AddressModel
    {
        public string Base58Address { get; set; }

        public string Chain { get; set; }

        public uint Index { get; set; }
    }
}
