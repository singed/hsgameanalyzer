using System;
using AutoMapper;
using HSGameAnalyzer.Mappings;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Newtonsoft.Json;
using Owin;
using Topshelf;

namespace HSGameAnalyzer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<AService>(s =>
                {
                    s.ConstructUsing(name => new AService());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription("HSService");
                x.SetDisplayName("HSService");
                x.SetServiceName("HSService");
            });


            //EventConsumer c = new EventConsumer();
        }
    }

    public class AService : ServiceControl
    {
        private IDisposable SignalR;

        public bool Start(HostControl hostControl)
        {
            throw new NotImplementedException();
        }

        public bool Stop(HostControl hostControl)
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            var url = "http://localhost:8088";
            SignalR = WebApp.Start(url);
            var c = new EventConsumer();
            c.ConsumeDat();
        }

        public void Stop()
        {
            SignalR.Dispose();
        }
    }

    internal class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Map("", map =>
            {
                Mapper.Initialize(cfg => { cfg.AddProfile(new MappingsConfiguration()); });

                var settings = new JsonSerializerSettings();
                settings.ContractResolver = new SignalRContractResolver();
                var serializer = JsonSerializer.Create(settings);
                GlobalHost.DependencyResolver.Register(typeof (JsonSerializer), () => serializer);

                var hubConfiguration = new HubConfiguration();
                hubConfiguration.EnableDetailedErrors = true;
                map.UseCors(CorsOptions.AllowAll);
                map.RunSignalR(hubConfiguration);
            });
        }
    }
}