using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QueueMessageSender.Models
{
    public class TokenModel
    {
        [Key]
        public int Id { get; set; }
        public string RefreshToken { get; set; }
        [ForeignKey("idUser")]
        public UserModel User { get; set; }
    }
}
