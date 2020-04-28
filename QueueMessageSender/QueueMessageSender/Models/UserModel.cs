using QueueMessageSender.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QueueMessageSender.Logic.Models
{
    public class UserModel
    {
        public UserModel()
        {
            Tokens = new HashSet<TokenModel>();
        }

        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public virtual ICollection<TokenModel> Tokens { get; set; }
    }
}
