using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using RatAppAPI.Models;
using RatAppAPI.App_Start;

namespace RatAppAPI.Controllers
{
    [Route("sightings")]
    public class SightingController : ApiController
    {
        DBStorage db;
        AuthManager auth;

        public SightingController()
        {
            db = new DBStorage();
            auth = AuthManager.GetInstance();
        }

        [Route("sightings")]
        [HttpGet]
        public IEnumerable<Sighting> GetAll()
        {
            return db.GetAllSightings();
        }

        [Route("sightings/{id}", Name = "GetSighting")]
        [HttpGet]
        [ResponseType(typeof(Sighting))]
        public HttpResponseMessage Get(int id)
        {
            Sighting sighting = db.GetSightingById(id);

            if (sighting == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, sighting);
            }
        }

        [Route("sightings/report")]
        [HttpPost]
        public IHttpActionResult CreateSighting([FromBody]Sighting sighting, [FromUri] string token = "")
        {
            if (auth.ValidateToken(token) == null)
                return Unauthorized();

            if (sighting == null)
                return BadRequest("Improperly formated sighting object");

            Sighting result = db.InsertSighting(sighting);

            if (result != null)
                return CreatedAtRoute("GetSighting", new { id = result.ID }, result);

            return BadRequest("Unknown error");
        }

        [Route("sightings/delete/{id}")]
        [HttpDelete]
        public IHttpActionResult DeleteSighting(int id, [FromUri] string token = "")
        {
            User requestUser = auth.ValidateToken(token);
            if ((requestUser == null) || (requestUser.Role != Models.User.Roles.Admin))
                return Unauthorized();

            if (db.DeleteSighting(id))
                return Ok();

            return NotFound();
        }
    }
}