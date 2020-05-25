using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QueueMessageSender.Models
{
    [Table("Tokens")]
    public class TokenModel
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(25)]
        public string RefreshToken { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public UserModel User { get; set; }
    }
}
