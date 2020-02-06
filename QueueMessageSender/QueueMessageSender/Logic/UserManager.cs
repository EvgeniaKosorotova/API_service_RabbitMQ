using QueueMessageSender.Logic.Models;
using System;
using System.Collections.Generic;

namespace QueueMessageSender.Logic
{
    /// <summary>
    /// Class to manage user list.
    /// </summary>
    public class UserManager
    {
        private static UserManager _instance = null;
        private readonly List<UserModel> users = new List<UserModel>();
        private readonly string defaultRefreshToken = "";

        public static UserManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new UserManager();
                return _instance;
            }
        }
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
                    RefreshToken = defaultRefreshToken
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
            if (token != null && token != defaultRefreshToken)
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
