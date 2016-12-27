using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HSCore.Entities;
using HSDeckCollector.Managers;
using MongoRepository;

namespace HSDeckCollector.Collectors
{
    public class HsTopDecksCollector
    {
        const string SiteUrl = "http://www.hearthstonetopdecks.com/";
        private readonly NavigationManager _navigationManager;
        private readonly ActionManager _actionManager;
        private readonly MongoRepository<HSCard> _cardRepository;
        private readonly MongoRepository<HSDeck> _deckRepository;

        public HsTopDecksCollector(NavigationManager navigationManager, ActionManager actionManager)
        {
            _cardRepository = new MongoRepository<HSCard>();
            _deckRepository = new MongoRepository<HSDeck>();
            _navigationManager = navigationManager;
            _actionManager = actionManager;

        }

        public void Collect()
        {
            try
            {
                int decksCollected = 0;
                _navigationManager.GoToPage(SiteUrl);

                var decksLinks =
                    _actionManager.TryFindElementsByJquery(".recent-header ul li a")
                        .Select(x => x.GetAttribute("href"))
                        .ToList();

                var decksInBase = _deckRepository.Where(d => decksLinks.Contains(d.Link)).Select(x => x.Link).ToList();

                foreach (var link in decksLinks.Where(x => !decksInBase.Contains(x)))
                {
                    try
                    {
                        if (!link.Contains(DateTime.Now.ToString("MMMM").ToLower())) // only get decks from this month
                            continue;

                        _navigationManager.GoToPage(link);
                        var deckInfo = _actionManager.TryFindElementsByJquery(".deck-info a");

                        // deck header
                        HSDeck deck = new HSDeck();
                        deck.Class = deckInfo[0].Text;
                        deck.Link = link;
                        deck.Type = deckInfo[1].Text;
                        deck.Name = _actionManager.TryFindElementByJquery("h1.entry-title").Text;
                        if (deck.Name.ToLower().Contains("wild"))
                            continue;

                        deck.Date = DateTime.Now;

                        // get cards names
                        var cardsNames = _actionManager.TryFindElementsByJquery(".deck-class li a span.card-name").Select(
                            x =>
                            {
                                var str = x.Text.Replace("’", "'");
                                return str.ToUpper();
                            }).ToList();
                        var cardsCount = _actionManager.TryFindElementsByJquery(".deck-class li a span.card-count").Select(x => x.Text).ToList();

                        var listofCards = _cardRepository.Where(x => cardsNames.Contains(x.name)).ToList();

                        IList<HSCard> cardsInDeck = new List<HSCard>();
                        for (int i = 0; i < cardsNames.Count; i++)
                        {
                            var card = listofCards.FirstOrDefault(elm => elm.name == cardsNames[i]);
                            if (card != null)
                            {
                                card.count = int.Parse(cardsCount[i]);
                                cardsInDeck.Add(card);
                            }
                            else
                            {
                                Console.WriteLine("Card " + cardsNames[i] + " not found in the database; Deck is not collected!");
                                throw new CardNotFoundException();
                            }

                        }
                        deck.Cards = cardsInDeck;
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
                Console.WriteLine("!!!!! " + ex.Message);
            }
        }
    }
}
