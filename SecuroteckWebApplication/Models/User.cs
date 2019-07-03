using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml;

namespace SecuroteckWebApplication.Models
{
    public class User
    {
        #region Task2
        // TODO: Create a User Class for use with Entity Framework
        // Note that you can use the [key] attribute to set your ApiKey Guid as the primary key 
        [Key]
        public string ApiKey { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        public ICollection<Log> Log { get; set; }
        public User()
        {
        }
        #endregion
    }

    #region Task13?
    // TODO: You may find it useful to add code here for Logging
    public class Log
    {
        [Key]
        public string LogID { get; set; }
        public string LogString { get; set; }
        public DateTime LogDateTime { get; set; }
        public Log(string pString, DateTime pDateTime)
        {
            LogString = pString;
            LogDateTime = pDateTime;
        }
    }
    #endregion

    public class UserDatabaseAccess
    {
        #region Task3 
        // TODO: Make methods which allow us to read from/write to the database 
        
        public string NewUser(string pUserName)
        {            
            Guid guid = Guid.NewGuid();
            string key = guid.ToString();
            using (var Context = new UserContext())
            {
                if(Context.Users.Count() > 0)
                {
                    Context.Users.Add(new User() { ApiKey = key, UserName = pUserName, Role = "User" });
                    string logString = "Created User";
                    Context.Logs.Add(new Log(logString, DateTime.UtcNow));
                }
                else
                {
                    Context.Users.Add(new User() { ApiKey = key, UserName = pUserName, Role = "Admin" });
                    string logString = "Created User";
                    Context.Logs.Add(new Log(logString, DateTime.UtcNow));
                }
                
                Context.SaveChanges();
            }
            return key;
        }

        public bool CheckUser(string pApiKey)
        {
            bool checkState = false;
            using (var Context = new UserContext())
            {
                User testUser = Context.Users.FirstOrDefault(User => User.ApiKey == pApiKey);
                if (testUser != null)
                {
                    checkState = true;

                    return checkState;
                }
                else
                {
                    return checkState;
                }
            }
        }

        public bool CheckUser(string pApiKey, string pUserName)
        {
            bool checkState = false;
            using (var Context = new UserContext())
            {
                User testUser = Context.Users.FirstOrDefault(User => User.ApiKey == pApiKey);
                if (testUser != null)
                {
                    if (testUser.UserName == pUserName)
                    {
                        checkState = true;
                        return checkState;
                    }
                    else
                    {
                        return checkState;
                    }
                }
                else
                {
                    return checkState;
                }
            }
        }

        public bool CheckUserbyName(string pUserName)
        {
            bool checkState = false;
            using (var Context = new UserContext())
            {
                User testUser = Context.Users.FirstOrDefault(User => User.UserName == pUserName);
                if (testUser != null)
                {
                    if (testUser.UserName == pUserName)
                    {
                        checkState = true;
                        return checkState;
                    }
                    else
                    {
                        return checkState;
                    }
                }
                else
                {
                    return checkState;
                }
            }
        }

        public User GetUser(string pApiKey)
        {
            using (var Context = new UserContext())
            {
                User testUser = Context.Users.FirstOrDefault(User => User.ApiKey == pApiKey);
                if (testUser != null)
                {
                    return testUser;
                }
                else
                {
                    return null;
                }
            }
        }

        public void DeleteUser(string pApiKey)
        {
            using (var Context = new UserContext())
            {
                User user = Context.Users.FirstOrDefault(User => User.ApiKey == pApiKey);
                Context.Users.Remove(user);
                Context.SaveChanges();
            }
        }

        public bool ChangeRole(string pUsername, string pRole)
        {
            using (var Context = new UserContext())
            {
                User user = Context.Users.FirstOrDefault(User => User.UserName == pUsername);
                if (user != null)
                {
                    user.Role = pRole;
                    Context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion
    }


}