using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QueueMessageSender.Data.Models
{
    [Table("Roles")]
    public class RoleObj
    {
        public RoleObj()
        {
            this.Users = new HashSet<UserObj>();
        }
        [Key]
        public int Id { get; set; }
        [MaxLength(20)]
        public string Role { get; set; }
        public virtual ICollection<UserObj> Users { get; set; }
    }
}
