using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OpenchainPlatform.API.ViewModels
{
    public class CreateTransactionViewModel
    {
        public CreateTransactionViewModel()
        {
            Recipients = new List<TransactionRecipientViewModel>();
        }

        [Required]
        [JsonProperty("asset_id")]
        public string AssetId { get; set; }

        [Required]
        [JsonProperty("account_id")]
        public string AccountId { get; set; }

        [Required]
        [JsonProperty("passphrase")]
        public string Passphrase { get; set; }

        [Required]
        [JsonProperty("recipients")]
        public List<TransactionRecipientViewModel> Recipients { get; set; }
    }
}
