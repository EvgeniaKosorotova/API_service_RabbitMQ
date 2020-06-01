using System;

namespace QueueMessageSender.Models
{
    public class AuthenticationResultModel
    {
        public string AccessToken { get; set; }

        /// <summary>
        /// The lifetime of the token access
        /// </summary>
        public TimeSpan LifeTime { get; set; }
        public string RefreshToken { get; set; }
        public int RoleId { get; set; }
    }
}
