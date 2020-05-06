using System.ComponentModel.DataAnnotations;

namespace QueueMessageSender.Models
{
    public class TokenModel
    {
        [Key]
        public int Id { get; set; }
        public int IdUser { get; set; }
        public string RefreshToken { get; set; }

        public virtual UserModel User { get; set; }
    }
}
