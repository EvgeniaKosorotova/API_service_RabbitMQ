using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QueueMessageSender.Models
{
    public class UserModel
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public virtual ICollection<TokenModel> Tokens { get; set; }
    }
}
