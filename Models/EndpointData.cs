using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Send.Models
{
    public class EndpointData
    {
        [Required]
        //[MaxLength(30)]
        [RegularExpression(@"^[a-zA-Z0-9]{1}[a-zA-Z0-9_]{0,29}$")]
        [JsonPropertyName("exchange")]
        public string NameExchange { get; set; }

        [Required]
        //[StringLength(30)]
        [RegularExpression(@"^[a-zA-Z0-9]{1}[a-zA-Z0-9_.]{0,29}$")]
        [JsonPropertyName("key")]
        public string RoutingKey { get; set; }

        [JsonPropertyName("message")]
        public object Message { get; set; }
    }
}
