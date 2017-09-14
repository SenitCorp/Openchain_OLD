using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenchainPlatform.API.ViewModels
{
    public class TransactionEntryViewModel
    {
        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("value")]
        public long Value { get; set; }
    }
}
