using System.ComponentModel.DataAnnotations;

namespace QueueMessageSender.Logic.Models
{
    public class UserModel
    {
        [Key]
        public string Username { get; set; }
        public string Password { get; set; }
        public string RefreshToken { get; set; }
    }
}
