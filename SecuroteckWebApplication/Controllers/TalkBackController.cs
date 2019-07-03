using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SecuroteckWebApplication.Controllers
{
    public class TalkBackController : ApiController
    {
        [ActionName("Hello")]
        public HttpResponseMessage Get()
        {
            #region TASK1
            HttpResponseMessage responseMessage = this.Request.CreateResponse(HttpStatusCode.OK, "Hello World");
            return responseMessage;
            #endregion
        }

        [ActionName("Sort")]
        public HttpResponseMessage Get([FromUri]int[] integers)
        {
            #region TASK1            
            // TODO: 
            // sort the integers into ascending order
            // send the integers back as the api/talkback/sort response
            if(!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            else
            {
                Array.Sort(integers);
                return Request.CreateResponse(HttpStatusCode.OK, integers);
            }
                      
            #endregion
        }

    }
}
