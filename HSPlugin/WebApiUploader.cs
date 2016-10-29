using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using HSCore.Entities;

namespace HSPlugin
{
    public class WebApiUploader
    {
        public void Send(string message)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:6001/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpContent content = new StringContent(message, Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync("api/game", content).Result;
            if (response.IsSuccessStatusCode)
            {
                var yourcustomobjects = response.Content.ReadAsStringAsync().Result;
            }
            else
            {
                Trace.WriteLine("something wrong");
            }
        }
    }
}
