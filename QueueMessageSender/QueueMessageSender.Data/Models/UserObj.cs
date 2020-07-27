using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QueueMessageSender.Data.Models
{
    [Table("Users")]
    public class UserObj
    {
        public UserObj()
        {
            this.Tokens = new HashSet<TokenObj>();
        }
        [Key]
        public int Id { get; set; }
        [MaxLength(20)]
        public string Username { get; set; }
        [MaxLength(50)]
        public string Password { get; set; }
        [ForeignKey("RoleId")]
        public int RoleId { get; set; }
        public virtual RoleObj Role { get; set; }
        public virtual ICollection<TokenObj> Tokens { get; set; }
    }
}
