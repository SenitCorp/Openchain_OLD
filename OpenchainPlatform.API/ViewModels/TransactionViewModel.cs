using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenchainPlatform.API.ViewModels
{
    public class TransactionViewModel
    {
        public TransactionViewModel()
        {
            Entries = new List<TransactionEntryViewModel>();
        }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("sequence_id")]
        public string SequenceId { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("entries")]
        public List<TransactionEntryViewModel> Entries { get; set; }
    }
}
