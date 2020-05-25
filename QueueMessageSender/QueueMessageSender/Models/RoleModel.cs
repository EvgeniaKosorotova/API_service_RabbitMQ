using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QueueMessageSender.Models
{
    [Table("Roles")]
    public class RoleModel
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(20)]
        public string Role { get; set; }
    }
}
