using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenchainPlatform.Core.Services.Models
{
    public class AssetModel
    {
        public string Path { get; set; }

        public long Balance { get; set; }

        public uint Index { get; set; }

        public string Chain { get; set; }

        public string AccountAddress { get; set; }
    }
}
