namespace QueueMessageSender.Controllers.Models
{
    public class LoginResultModel
    {
        public bool Successful { get; set; }
        public string Error { get; set; }
        public string Token { get; set; }
    }
}
