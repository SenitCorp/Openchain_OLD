using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OpenchainPlatform.API.ViewModels
{
    public class CreateAccountAddressViewModel
    {
        [Required]
        [JsonProperty("passphrase")]
        public string Passphrase { get; set; }

        [JsonProperty("account_id")]
        public string AccountId { get; set; }
    }
}
