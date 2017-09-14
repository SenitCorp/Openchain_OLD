using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OpenchainPlatform.API.ViewModels
{
    public class AccountAddressViewModel
    {
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("account_id")]
        public string AccountId { get; set; }

        [JsonProperty("chain")]
        public string Chain { get; set; }

        [JsonProperty("index")]
        public uint Index { get; set; }
    }
}
