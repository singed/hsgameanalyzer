using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HSCore.Entities;
using HSCore.Extensions;
using HSDeckCollector.Managers;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoRepository;

namespace HSDeckCollector.Collectors
{
    public class HearthpwnCollector
    {
        const string SiteUrl = "http://www.hearthpwn.com/";
        private readonly NavigationManager _navigationManager;
        private readonly ActionManager _actionManager;
        private readonly MongoRepository<HSCard> _cardRepository;
        private readonly MongoRepository<HSDeck> _deckRepository;
        public HearthpwnCollector(NavigationManager navigationManager, ActionManager actionManager)
        {
            _cardRepository = new MongoRepository<HSCard>();
            _deckRepository = new MongoRepository<HSDeck>();
            _navigationManager = navigationManager;
            _actionManager = actionManager;
        }

        public void Collect()
        {
            int decksCollected = 0;
            _navigationManager.GoToPage(SiteUrl);

            // document.querySelectorAll('ul.class-tabs li')
            var classListCount = _actionManager.TryFindElementsByJs(".class-tabs li a span").Skip(1).ToList().Count;
            for (int i = 0; i < classListCount; i++)
            {
                int errorsCount = 0;
                try
                {
                    if (_navigationManager.CurrentUrl != SiteUrl)
                    {
                        _navigationManager.GoToPage(SiteUrl);
                    }
                    int counter = i + 2;
                    //document.querySelectorAll('.decks li a')
                    var dclass = _actionManager.TryFindElementByJs(".class-tabs li a[data-class-id='" + counter + "']");
                    dclass.Click();

                    Thread.Sleep(3000);

                    
                    // check if deck is already exist
                    // by name
                    var decksLinks = _actionManager.TryFindElementsByJquery(".decks li a").Select(x => x.GetAttribute("href")).Take(6).ToList();
                    var decksInBase = _deckRepository.Where(d => decksLinks.Contains(d.Link)).Select(x => x.Link).ToList();

                    string deckClass = dclass.Text;
                    Console.WriteLine("=== Collecting {0} ====", deckClass);
                    foreach (var link in decksLinks.Where(x => !decksInBase.Contains(x)))
                    {
                        try
                        {
                            HSDeck deck = new HSDeck();
                            deck.Class = deckClass;
                            deck.Link = link;
                            deck.Date = DateTime.Now;
                            //deck.SiteName = SiteUrl;
                            _navigationManager.GoToPage(link);

                            string deckName = _actionManager.TryFindElementByJquery(".deck-detail h2.deck-title").Text;
                            string deckType = _actionManager.TryFindElementsByJquery(".deck-details li .deck-type")[1].Text;

                            deck.Name = deckName;
                            deck.Type = deckType;

                            var exportDdl = _actionManager.TryFindElementByJquery(".t-export-deck select");
                            exportDdl.Click();
                            var option = _actionManager.TryFindElementsByJquery(".t-export-deck select option")[2];
                            option.Click();
                            var exportWindow = _actionManager.TryFindElementByJquery(".deck-export-area");

                            Dictionary<string, int> importedCards = GetCardsFromCockaTrice(exportWindow.Text.ToUpper());
                            var cardsNames = importedCards.Select(c => c.Key.ToUpper()).ToList();
                            var listofCards = _cardRepository.Where(x => cardsNames.Contains(x.name)).ToList();

                            deck.Cards = GetCardsInDeckWithCount(listofCards, importedCards);
                            _deckRepository.Add(deck);
                            decksCollected++;
                            Console.WriteLine("Collected : " + deck.Name);
                        }
                        catch (CardNotFoundException ex)
                        {
                            continue;
                        }
                      
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("!!!! " + ex.Message);
                    Console.WriteLine("trying  again " + errorsCount);
                    errorsCount++;
                    if (errorsCount == 3)
                        continue;
                    i--;
                }
            }
            Console.WriteLine("HearthpwnCollector Decks collected: " + decksCollected);
        }

        private static IList<HSCard> GetCardsInDeckWithCount(IList<HSCard> cards, Dictionary<string, int> importedCards)
        {
            var list = new List<HSCard>();
            importedCards.ForEach(impCard =>
            {
                var card = cards.FirstOrDefault(x => x.name == impCard.Key);
                if (card != null)
                {
                    card.count = impCard.Value;
                    list.Add(card);
                }
                else
                {
                    Console.WriteLine("Card " + impCard.Key + " not found in the database; Deck is not collected!");
                    throw new CardNotFoundException();
                }
            });

            return list;
        }

        private static Dictionary<string, int> GetCardsFromCockaTrice(string input)
        {
            string[] stringSeparators = new string[] { "\r\n" };
            string[] lines = input.Split(stringSeparators, StringSplitOptions.None);
            var dict = new Dictionary<string, int>();
            foreach (var item in lines)
            {
                dict.Add(item.Substring(2, item.Length - 2), int.Parse(item.Substring(0, 1)));

            }
            return dict;
        }
    }
}
