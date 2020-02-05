namespace QueueMessageSender.Controllers.Models
{
    public class AuthenticationResultModel
    {
        public bool Successful { get; set; }
        public string Error { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
