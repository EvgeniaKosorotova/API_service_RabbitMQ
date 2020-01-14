using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace QueueMessageSender.Models
{
    /// <summary>
    /// The model of the data sent in the POST request.
    /// </summary>
    public class EndpointData
    {
        private string nameExchange;

        [Required]
        [JsonPropertyName("exchange")]
        public string NameExchange
        {
            get => nameExchange;
            set
            {
                nameExchange = "123";
                //Regex r = new Regex(@"^[a-zA-Z0-9]{1}[a-zA-Z0-9_]{0,29}$");
                //nameExchange = r.IsMatch(value) ? value : null;
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
                routingKey = "123";
                //Regex r = new Regex(@"^[a-zA-Z0-9]{1}[a-zA-Z0-9_.]{0,29}$");
                //routingKey = r.IsMatch(value) ? value : null;
            }
        }

        public object Message { get; set; }
    }
}
