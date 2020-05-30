using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QueueMessageSender.Models
{
    [Table("Users")]
    public class UserModel
    {
        public UserModel()
        {
            this.Tokens = new HashSet<TokenModel>();
        }
        [Key]
        public int Id { get; set; }
        [MaxLength(20)]
        public string Username { get; set; }
        [MaxLength(50)]
        public string Password { get; set; }
        [ForeignKey("RoleId")]
        public int RoleId { get; set; }
        public virtual RoleModel Role { get; set; }
        public virtual ICollection<TokenModel> Tokens { get; set; }
    }
}
