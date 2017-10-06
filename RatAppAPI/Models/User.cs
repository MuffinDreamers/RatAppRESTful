using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace RatAppAPI.Models
{
    public class User
    {
        public enum Roles
        {
            User,
            Admin
        }

        [JsonProperty("id")]
        public int ID { get; set; }
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
        [JsonProperty("role")]
        [JsonConverter(typeof(StringEnumConverter))]
        public Roles Role { get; set; }

        [JsonConstructor]
        public User(int ID, string username, string password, Roles role = Roles.User)
        {
            this.ID = ID;
            this.Username = username;
            this.Password = password;
            this.Role = role;
        }

        public User(Object[] values)
        {
            if (values.Length != 4)
                throw new InvalidOperationException("User can only be constructed by an array with 3 values");
            if (values[0].GetType() != typeof(int))
                throw new InvalidOperationException("First value of array must be the id int");
            if (values[1].GetType() != typeof(string))
                throw new InvalidOperationException("Second value of array must be the username string");
            if (values[2].GetType() != typeof(string))
                throw new InvalidOperationException("Third value of array must be the password string");
            if ((values[3].GetType() != typeof(Roles)) && (values[3].GetType() != typeof(string)))
                throw new InvalidOperationException("Fourth value of array must be the role enum or string");
            ID = Convert.ToInt32(values[0]);
            Username = Convert.ToString(values[1]);
            Password = Convert.ToString(values[2]);
            Role = (Roles)Enum.Parse(typeof(Roles), Convert.ToString(values[3]));

            Debug.WriteLine("Created " + ToString());
        }

        public User(User old)
        {
            ID = old.ID;
            Username = string.Copy(old.Username);
            Password = string.Copy(old.Password);
            Role = old.Role;
        }

        override public string ToString()
        {
            return "(" + Username + ", " + Password + ", " + Role + ")";
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(User))
                return false;
            User u = (User)obj;

            if (u.ID != ID)
                return false;
            if (u.Username != Username)
                return false;
            if (u.Password != Password)
                return false;
            if (u.Role != Role)
                return false;

            return true;
        }
    }
}