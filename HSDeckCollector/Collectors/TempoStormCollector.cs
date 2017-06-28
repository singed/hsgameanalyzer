using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HSCore.Entities;
using HSDeckCollector.Managers;
using MongoRepository;
using OpenQA.Selenium;

namespace HSDeckCollector.Collectors
{
    public class TempoStormCollector
    {
        const string SiteUrl = "https://tempostorm.com/hearthstone/meta-snapshot/standard/2017-06-05";

        private readonly NavigationManager _navigationManager;
        private readonly ActionManager _actionManager;
        private readonly MongoRepository<HSCard> _cardRepository;
        private readonly MongoRepository<HSDeck> _deckRepository;

        public TempoStormCollector(NavigationManager navigationManager, ActionManager actionManager)
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

            var tiers = _actionManager.TryFindElementsByJquery(".tiers .tier");

            foreach (var tier in tiers)
            {
                string tierId = "#tier" + tier.GetAttribute("id").Substring(4, 1);
                // open tier
                var tierButton = _actionManager.TryFindElementByJquery(tierId + " button");
                tierButton.Click();
                var decksList = _actionManager.TryFindElementsByJquery(tierId + " .tier-body .tier-deck");
                for (int index = 0; index < decksList.Count; index++)
                {
                    var buttonId = $"#deck{index + 1} button";
                    var button = _actionManager.TryFindElementByJquery(buttonId);
                    button.Click();
                    ParentManager.Driver.SwitchTo().Window(ParentManager.Driver.WindowHandles.Last());
                    var buttonPopupElm = _actionManager.TryFindElementByJquery(".db-deck-copy button");
                    buttonPopupElm.Click();

                    var decktext = _actionManager.TryFindElementByJquery(".modal-dialog textarea").GetAttribute("value");
                    string deckUrl = _navigationManager.CurrentUrl;
                    ParentManager.Driver.Close();
                    SaveDeck(decktext, deckUrl);
                    ParentManager.Driver.SwitchTo().Window(ParentManager.Driver.WindowHandles.First());
                }
            }
        }

        private void SaveDeck(string deckText, string deckUrl)
        {
            var dbDeck = _deckRepository.Where(d => d.Link == deckText).Select(x => x.Link).FirstOrDefault();
            if (dbDeck == null)
            {
                HSDeck deck = new HSDeck();
                deck.Name = GetName(deckText);
                deck.Class = GetClass(deckText);
                deck.Link = deckUrl;
                deck.Cards = GetCards(deckText);
                deck.Date = DateTime.Now;
                _deckRepository.Add(deck);
            }
        }

        private IList<HSCard> GetCards(string text)
        {
            var lines = text.Split('#');
            Dictionary<string, int> importedCards = new Dictionary<string, int>();
            for (int ind = 8; ind < lines.Length; ind++)
            {
                if (string.IsNullOrEmpty(lines[ind]))
                {
                    break;
                }
                string line = lines[ind];
                
                string cardname = line.Substring(line.IndexOf(")")+1).Trim();
                try
                {
                    int cardCount = int.Parse(line.Split(' ').FirstOrDefault(x => x.Contains("x")).Substring(0, 1));
                    importedCards.Add(cardname, cardCount);
                }
                catch (Exception e)
                {
                    break;
                }
            }

            var cardsNames = importedCards.Select(c => c.Key.ToUpper()).ToList();
            var listofCards = _cardRepository.Where(x => cardsNames.Contains(x.name)).ToList();
            var names = listofCards.Select(x => x.name).ToList();
            return listofCards;
        }

        private string GetName(string text)
        {
            return text.Split('#')[3].Split('-')[0].Trim();
        }

        private string GetClass(string text)
        {
            var lines = text.Split('#');
            return lines.FirstOrDefault(x => x.Contains("Class")).Split(':')[1].Trim();
        }
    }
}
