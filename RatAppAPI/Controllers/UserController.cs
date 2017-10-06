using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using RatAppAPI.Models;
using System.Diagnostics;
using RatAppAPI.App_Start;
using System.Data.Objects;
using System.Web.Http.Description;
using System.Text;

namespace RatAppAPI.Controllers
{
    [Route("user")]
    public class UserController : ApiController
    {
        DBStorage db;
        AuthManager auth;

        public UserController()
        {
            db = new DBStorage();
            auth = AuthManager.GetInstance();
        }

        [Route("user/{id}", Name = "GetUser")]
        [HttpGet]
        [ResponseType(typeof(User))]
        public HttpResponseMessage GetUser(int id, [FromUri] string token = "")
        {
            if (auth.ValidateToken(token) == null)
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);

            User user = db.GetUserById(id);
            if (user == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            else
            {
                User noPassword = new User(user);
                noPassword.Password = "";
                return Request.CreateResponse(HttpStatusCode.OK, noPassword);
            }

        }

        [Route("user/register")]
        [HttpPost]
        public IHttpActionResult CreateUser([FromBody]User user)
        {
            if (user == null)
                return BadRequest();

            User result = db.InsertUser(user);

            result.Password = "";

            if(result != null)
                return CreatedAtRoute("GetUser", new { id = result.ID }, result);

            return Conflict();
        }

        //Edits the user settings. Uses the User ID to know which user to edit (User ID cannot be changed)
        [Route("user/edit")]
        [HttpPut]
        public IHttpActionResult EditUser([FromBody]User user, [FromUri] string token = "")
        {
            User requestUser = auth.ValidateToken(token);
            if ((requestUser == null) || (requestUser.Role != Models.User.Roles.Admin))
                return Unauthorized();
            //TODO
            return BadRequest("Unimplemented");
        }

        [Route("user/delete/{id}")]
        [HttpDelete]
        public IHttpActionResult DeleteUser(int id, [FromUri] string token = "")
        {
            User requestUser = auth.ValidateToken(token);
            if ((requestUser == null) || (requestUser.Role != Models.User.Roles.Admin))
                return Unauthorized();

            if (db.DeleteUser(id))
                return Ok();

            return NotFound();
        }


        [Route("user/auth")]
        [HttpPost]
        public HttpResponseMessage AuthenticateUser([FromBody]User user)
        {
            if (user == null)
                return new HttpResponseMessage(HttpStatusCode.BadRequest);

            //Username and password match
            string token = auth.AuthenticateUser(user);
            if (token == null)
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);

            Debug.WriteLine($"Successfully authenticated {user.Username}. Token: {token}");
            return Request.CreateResponse(HttpStatusCode.OK, token);
        }

        [Route("user/logout")]
        [HttpGet]
        public IHttpActionResult Logout([FromUri] string token = "")
        {
            bool success = auth.RevokeToken(token);
            if (success)
                return Ok();
            else
                return Unauthorized();
        }
    }
}