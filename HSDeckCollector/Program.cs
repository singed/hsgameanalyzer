using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HSCore.Entities;
using HSDeckCollector.Collectors;
using HSDeckCollector.Core;
using HSDeckCollector.Managers;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoRepository;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;

namespace HSDeckCollector
{
    class Program
    {
        static void Main(string[] args)
        {
            /*  var _cardRepository = new MongoRepository<HSCard>();

              var cards = new Dictionary<string, int> { { "Onyxia", 1 } }.Select(x=>x.Key).ToList();
              var listofCards = _cardRepository.Where(x => cards.Contains(x.name)).ToList();*/
            //FirefoxDriver
            Browser browser = new Browser(typeof(FirefoxDriver));
            NavigationManager navigationManager = new NavigationManager(browser, "");
            ActionManager actionManager = new ActionManager(browser);

            HearthpwnCollector hearthpwnCollector = new HearthpwnCollector(navigationManager, actionManager);
            hearthpwnCollector.Collect();

            HsTopDecksCollector hsTopDecksCollector = new HsTopDecksCollector(navigationManager, actionManager);
            hsTopDecksCollector.Collect();

            Console.ReadKey();
        }
    }

}
