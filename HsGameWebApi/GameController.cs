using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using HSCore;
using HSCore.Entities;

namespace HsGameWebApi
{
    public class GameController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Get()
        {
            BusPublisher pub = new BusPublisher();
            pub.PublishMessage(new HsGameMessage(HSGameEventTypes.OnGameStart));
            return Request.CreateResponse(HttpStatusCode.OK, "heelo");
        }
    }
}