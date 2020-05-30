using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QueueMessageSender.Models
{
    [Table("Roles")]
    public class RoleModel
    {
        public RoleModel()
        {
            this.Users = new HashSet<UserModel>();
        }
        [Key]
        public int Id { get; set; }
        [MaxLength(20)]
        public string Role { get; set; }
        public virtual ICollection<UserModel> Users { get; set; }
    }
}
