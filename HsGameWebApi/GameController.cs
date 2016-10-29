using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using HSCore;
using HSCore.Entities;
using Newtonsoft.Json;

namespace HsGameWebApi
{
    public class GameController : ApiController
    {
        public HttpResponseMessage Post([FromBody]HsGameMessage message)
        {
            BusPublisher pub = new BusPublisher();
            pub.PublishMessage(message);
            return Request.CreateResponse(HttpStatusCode.OK, "ok");
        }
    }
}