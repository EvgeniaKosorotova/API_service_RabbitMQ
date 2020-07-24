using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QueueMessageSender.Data.Models
{
    [Table("Tokens")]
    public class TokenObj
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(25)]
        public string RefreshToken { get; set; }
        [ForeignKey("UserId")]
        public int UserId { get; set; }
        public virtual UserObj User { get; set; }
    }
}
