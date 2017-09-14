using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenchainPlatform.API.ViewModels
{
    public class TransactionRecipientViewModel
    {
        [JsonProperty("account_id")]
        public string AccountId { get; set; }

        [JsonProperty("amount")]
        public long Amount { get; set; }
    }
}
