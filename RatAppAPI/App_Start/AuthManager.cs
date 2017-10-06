using RatAppAPI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;

namespace RatAppAPI.App_Start
{
    public class AuthManager
    {
        private static AuthManager INSTANCE;
        private static Dictionary<string, User> validTokens;

        DBStorage db = new DBStorage();

        private AuthManager()
        {
            validTokens = new Dictionary<string, User>();
        }

        public string AuthenticateUser(User user)
        {
            User storedUser = db.GetUserByUsername(user.Username);

            if (storedUser == null)
                return null;

            if (storedUser.Password != user.Password)
                return null;

            //Authenticated
            string token = GenerateUserToken(storedUser.Username);
            validTokens.Add(token, new User(storedUser));
            Debug.WriteLine($"Registered token {token} for user {validTokens[token].Username}");
            return token;
        }

        public bool RevokeToken(string token)
        {
            if(ValidateToken(token) != null)
            {
                Debug.WriteLine($"Revoking token {token} for user {validTokens[token].Username}");
                return validTokens.Remove(token);
            }
            return false;
        }

        public bool RevokeToken(User user)
        {
            bool success = true;
            foreach(KeyValuePair<string, User> pair in validTokens)
            {
                if (pair.Value.Equals(user))
                {
                    Debug.WriteLine($"Revoking token {pair.Key} for user {user.Username}");
                    if (!validTokens.Remove(pair.Key))
                        success = false;
                }
            }

            return success;
        }

        public User ValidateToken(string token)
        {
            if (!validTokens.ContainsKey(token))
                return null;
            return validTokens[token];
        }

        private string GenerateUserToken(int userId)
        {
            byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            byte[] key = Guid.NewGuid().ToByteArray();
            byte[] user = BitConverter.GetBytes(userId);
            return Convert.ToBase64String(time.Concat(user).Concat(key).ToArray());
        }

        private string GenerateUserToken(string username)
        {
            byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            byte[] key = Guid.NewGuid().ToByteArray();
            byte[] user = Encoding.ASCII.GetBytes(username);
            return Convert.ToBase64String(time.Concat(user).Concat(key).ToArray());
        }

        public static AuthManager GetInstance()
        {
            if (INSTANCE == null)
                INSTANCE = new AuthManager();
            return INSTANCE;
        }
    }
}