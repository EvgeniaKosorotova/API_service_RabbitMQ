namespace QueueMessageSender.Controllers.Models
{
    public class AuthenticationResultModel
    {
        public bool Successful { get; set; }
        public string Error { get; set; }
        public string Token { get; set; }
    }
}
