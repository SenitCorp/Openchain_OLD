using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OpenchainPlatform.API.ViewModels
{
    public class CreateAccountViewModel
    {
        [Required]
        [JsonProperty("passphrase")]
        public string Passphrase { get; set; }

        [Required]
        [JsonProperty("asset_id")]
        public string AssetId { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }
    }
}
