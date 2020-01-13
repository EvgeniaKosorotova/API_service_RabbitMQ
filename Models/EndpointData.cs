using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace QueueMessageSender.Models
{
    public class EndpointData
    {
        private string nameExchange;

        [Required]
        [JsonPropertyName("exchange")]
        public string NameExchange 
        { 
            get => nameExchange; 
            set { 
                Regex r = new Regex(@"^[a-zA-Z0-9]{1}[a-zA-Z0-9_]{0,29}$");
                nameExchange = r.IsMatch(value) ? value : null;
            } 
        }

        private string routingKey;

        [Required]
        [JsonPropertyName("key")]
        public string RoutingKey 
        {
            get => routingKey;
            set
            {
                Regex r = new Regex(@"^[a-zA-Z0-9]{1}[a-zA-Z0-9_.]{0,29}$");
                routingKey = r.IsMatch(value) ? value : null;
            }
        }

        [JsonPropertyName("message")]
        public object Message { get; set; }
    }
}
