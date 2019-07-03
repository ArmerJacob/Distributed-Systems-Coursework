using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using SecuroteckWebApplication.Models;

namespace SecuroteckWebApplication.Controllers
{
    public class UserController : ApiController
    {
        UserDatabaseAccess userDatabase = new UserDatabaseAccess();
        [ActionName("new")]
        public HttpResponseMessage Get([FromUri]string username)
        {
            if(username != null)
            {
                bool user = userDatabase.CheckUserbyName(username);
                if(user == true)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,"True - User Does Exist! Did you mean to do a POST to create a new user?");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK,"False - User Does Not Exist! Did you mean to do a POST to create a new user?");
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, "False - User Does Not Exist! Did you mean to do a POST to create a new user?");
            }            
        }
        [ActionName("new")]
        public HttpResponseMessage Post([FromBody]string username)
        {
            if(username != null)
            {
                bool user = userDatabase.CheckUserbyName(username);
                if(user == true)
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "Oops. This username is already in use. Please try again with a new username.");
                }
                else
                {
                    string ApiKey = userDatabase.NewUser(username);
                    return Request.CreateResponse(HttpStatusCode.OK, ApiKey);
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Oops. Make sure your body contains a string with your username and your Content-Type is Content-Type:application/json");
            }
        }
        [APIAuthorise]
        [ActionName("removeuser")]
        public HttpResponseMessage Delete([FromUri] string username)
        {
            IEnumerable<string> key;
            Request.Headers.TryGetValues("x-api-key",out key);

            User user = userDatabase.GetUser(key.First());
            string logString = "Delete User";
            Log log = new Log(logString, DateTime.UtcNow);
            user.Log.Add(log);

            bool userCheck = userDatabase.CheckUser(key.First(), username);
            if(userCheck == true)
            {
                userDatabase.DeleteUser(key.First());
                return Request.CreateResponse(HttpStatusCode.OK, true);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, false);
            }      
        }

        [AdminRole] 
        [ActionName("changerole")]
        public HttpResponseMessage Post(JObject body)
        {
            string username = body.GetValue("username").ToString();
            string role = body.GetValue("role").ToString();
            if(role != "Admin" && role != "User")
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "NOT DONE: Role does not exist");
            }
            if(userDatabase.CheckUserbyName(username) == false)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "NOT DONE: Username does not exist");
            }
            bool roleCheck = userDatabase.ChangeRole(username, role);
            if (roleCheck == false)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "NOT DONE: An error occured");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, "DONE");
            }
        }
    }
}
