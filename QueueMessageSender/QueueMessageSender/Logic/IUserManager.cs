using QueueMessageSender.Logic.Models;

namespace QueueMessageSender.Logic
{
    /// <summary>
    /// Abstraction to manage user list.
    /// </summary>
    public interface IUserManager
    {
        bool Create(string username, string password);
        UserModel Read(string username);
        bool Update(string username, string accessToken, string refreshToken);
        bool Delete(string username);
        UserModel ReadByRefreshToken(string oldRefreshToken);
    }
}
