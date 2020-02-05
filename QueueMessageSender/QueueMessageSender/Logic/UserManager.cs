using QueueMessageSender.Logic.Models;
using System;
using System.Collections.Generic;

namespace QueueMessageSender.Logic
{
    /// <summary>
    /// Class to manage user list.
    /// </summary>
    public class UserManager : IUserManager
    {
        private readonly List<UserModel> users = new List<UserModel>();

        public UserManager()
        {
            Create("admin", "adminPassword");
        }

        public bool Create(string username, string password)
        {
            if (!users.Exists(u => u.Username.Contains(username))) 
            {
                users.Add(new UserModel 
                {
                    Username = username,
                    Password = password,
                    RefreshToken = ""
                });
                return true;
            }
            return false;
        }

        public bool Delete(string username)
        {
            return users.Remove(users.Find(u => u.Username.Contains(username)));
        }

        public UserModel Read(string username)
        {
            return users.Find(u => u.Username.Contains(username));
        }

        public UserModel ReadByRefreshToken(string token)
        {
            if (token != null)
            {
                return users.Find(u => u.RefreshToken.Contains(token));
            }
            else
            {
                return null;
            }
        }

        public bool Update(string username, string accessToken, string refreshToken)
        {
            if (users.Exists(u => u.Username.Contains(username)))
            {
                users.FindAll(u => u.Username == username)
                    .ForEach(x => 
                    {
                        x.RefreshToken = refreshToken;
                    });
                return true;
            }
            return false;
        }
    }
}
