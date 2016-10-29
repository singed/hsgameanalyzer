using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using HsGameWebApi;
using Microsoft.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace HsGameWebApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            
            HttpConfiguration config = new HttpConfiguration();

            // enable json formatter
          
            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());
            //config.Formatters.Add(new BrowserJsonFormatter());
            //config.Formatters.Add(new BsonMediaTypeFormatter());

            //Enabling Cross-Origin Requests
            config.EnableCors(new EnableCorsAttribute("*", "*", "*"));

            config.Routes.MapHttpRoute(
         name: "PostMethod",
         routeTemplate: "api/game/{message}",
         defaults: new { id = RouteParameter.Optional }
        );
            // routes config
            config.Routes.MapHttpRoute(
             name: "DefaultApi",
             routeTemplate: "api/{controller}/{id}",
             defaults: new { id = RouteParameter.Optional }
            );

            // fluent validation

            app.UseWebApi(config);
        }
    }
}