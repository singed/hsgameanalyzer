using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HSCore.Entities;

namespace HSPlugin
{
    public class WebApiUploader
    {
        public HttpResponseMessage SendGet()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:6001/");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //HttpContent content = new StringContent(message, Encoding.UTF8, "application/json");
                CancellationToken token = new CancellationToken();

                return client.GetAsync("api/event", token).Result;
            }
        }

        public void SendPost(string message)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:6001/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpContent content = new StringContent(message, Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync("api/game", content).Result;

            if (response.IsSuccessStatusCode)
            {
                
            }
            else
            {
                Trace.WriteLine("something wrong");
            }
        }
    }
}
