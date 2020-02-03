using System.ComponentModel.DataAnnotations;

namespace QueueMessageSender.Controllers.Models
{
    public class LoginModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        //public string Command { get; set; } = "";   // строка - команда, которая будет определять тип действий при обработке запроса

        //public TodoItem TodoItem { get; set; }      // значения свойств задачи в виде структуры задачи в формате json
    }
}
