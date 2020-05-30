using System.ComponentModel.DataAnnotations;

namespace QueueMessageSender.Models
{
    /// <summary>
    /// Credential model
    /// </summary>
    public class RegistrationModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
