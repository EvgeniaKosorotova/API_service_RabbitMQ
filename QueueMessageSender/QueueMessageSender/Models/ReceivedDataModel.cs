using System.ComponentModel.DataAnnotations;

namespace QueueMessageSender.Models
{
    /// <summary>
    /// The model of the data sent in the POST request.
    /// </summary>
    public class ReceivedDataModel
    {
        /// <summary>
        /// Name exchange
        /// </summary>
        [Required, StringLength(30)]
        [RegularExpression(@"^[a-zA-Z0-9]{1}[a-zA-Z0-9_]{0,}$")]
        public string Exchange { get; set; }

        /// <summary>
        /// Routing key
        /// </summary>
        [Required, StringLength(30)]
        [RegularExpression(@"^[a-zA-Z0-9]{1}[a-zA-Z0-9_.]{0,}$")]
        public string Key { get; set; }

        public object Message { get; set; }
    }
}
