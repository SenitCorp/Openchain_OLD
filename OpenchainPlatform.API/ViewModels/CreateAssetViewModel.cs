using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OpenchainPlatform.API.ViewModels
{
    public class CreateAssetViewModel
    {
        [Required]
        [JsonProperty("id")]
        public string Id { get; set; }

        [Required]
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("alias")]
        public string Alias { get; set; }

        [Required]
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [Required]
        [JsonProperty("precision")]
        public int Precision { get; set; }
    }
}
