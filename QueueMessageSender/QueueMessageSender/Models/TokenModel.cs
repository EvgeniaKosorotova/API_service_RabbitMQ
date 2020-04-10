using QueueMessageSender.Logic.Models;
using System.ComponentModel.DataAnnotations;

namespace QueueMessageSender.Models
{
    public class TokenModel
    {
        [Key]
        public int Id { get; set; }
        public int IdUser { get; set; }
        public string RefreshToken { get; set; }
    }
}
