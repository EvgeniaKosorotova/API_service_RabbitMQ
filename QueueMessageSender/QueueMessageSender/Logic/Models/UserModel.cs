using System.ComponentModel.DataAnnotations;

namespace QueueMessageSender.Logic.Models
{
    public class UserModel
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string RefreshToken { get; set; }
    }
}
