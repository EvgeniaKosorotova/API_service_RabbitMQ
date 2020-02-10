namespace QueueMessageSender.Controllers.Models
{
    public class AuthenticationResultModel
    {
        public string Error { get; set; }
        public string AccessToken { get; set; }

        /// <summary>
        /// The lifetime of the token access
        /// </summary>
        public string LifeTime { get; set; }
        public string RefreshToken { get; set; }
    }
}
