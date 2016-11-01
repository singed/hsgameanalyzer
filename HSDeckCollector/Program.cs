using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using HSCore;
using HSCore.Entities;
using HSDeckCollector.Collectors;
using HSDeckCollector.Core;
using HSDeckCollector.Managers;
using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;

namespace HSDeckCollector
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var browser = new Browser(typeof(ChromeDriver));
            var navigationManager = new NavigationManager(browser, "");
            var actionManager = new ActionManager(browser);

            var hearthpwnCollector = new HearthpwnCollector(navigationManager, actionManager);
            hearthpwnCollector.Collect();

            HsTopDecksCollector hsTopDecksCollector = new HsTopDecksCollector(navigationManager, actionManager);
            hsTopDecksCollector.Collect();

            Console.ReadKey();
        }
    }
}