using System;
using HSDeckCollector.Collectors;
using HSDeckCollector.Core;
using HSDeckCollector.Managers;
using OpenQA.Selenium.Chrome;

namespace HSDeckCollector
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var browser = new Browser(typeof (ChromeDriver));
            var navigationManager = new NavigationManager(browser, "");
            var actionManager = new ActionManager(browser);

            var hearthpwnCollector = new HearthpwnCollector(navigationManager, actionManager);
            hearthpwnCollector.Collect();

            /*  HsTopDecksCollector hsTopDecksCollector = new HsTopDecksCollector(navigationManager, actionManager);
            hsTopDecksCollector.Collect();*/

            Console.ReadKey();
        }
    }
}