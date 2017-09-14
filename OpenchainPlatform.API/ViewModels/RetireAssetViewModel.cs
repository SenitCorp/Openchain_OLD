using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OpenchainPlatform.API.ViewModels
{
    public class RetireAssetViewModel
    {
        [Required]
        [JsonProperty("account_id")]
        public string AccountId { get; set; }

        [Required]
        [JsonProperty("asset_id")]
        public string AssetId { get; set; }

        [Required]
        [JsonProperty("amount")]
        public long Amount { get; set; }
    }
}
