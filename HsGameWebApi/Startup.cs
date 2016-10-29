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
            JsonMediaTypeFormatter mediaTypeFormatter1 = new JsonMediaTypeFormatter();
            mediaTypeFormatter1.SerializerSettings.ContractResolver = (IContractResolver)new CamelCasePropertyNamesContractResolver();
            mediaTypeFormatter1.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            JsonMediaTypeFormatter mediaTypeFormatter2 = mediaTypeFormatter1;
            config.Formatters.Clear();
            config.Formatters.Add((MediaTypeFormatter)mediaTypeFormatter2);
            config.Formatters.Add((MediaTypeFormatter)new FormUrlEncodedMediaTypeFormatter());

            //Enabling Cross-Origin Requests
            config.EnableCors(new EnableCorsAttribute("*", "*", "*"));


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