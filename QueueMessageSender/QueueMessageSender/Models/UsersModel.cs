using System.Collections.Generic;

namespace QueueMessageSender.Models
{
    public class UsersModel<T>
    {
        public List<T> Result { get; set; }
    }
}
