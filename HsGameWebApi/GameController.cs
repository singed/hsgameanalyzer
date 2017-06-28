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
            pub.Dispose();
            return Request.CreateResponse(HttpStatusCode.OK, "ok");
        }
    }

    public class EventController : ApiController
    {
        public HttpResponseMessage Get()
        {
            BusPublisher pub = new BusPublisher();
            var msg = new HsGameMessage(HSGameEventTypes.OnGameStart) {
                Data = null
            };
            pub.PublishMessage(msg);
            pub.Dispose();
            return Request.CreateResponse(HttpStatusCode.OK, "ok");
        }
    }
}