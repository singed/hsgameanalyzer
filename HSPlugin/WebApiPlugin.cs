using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Hearthstone_Deck_Tracker.API;
using Hearthstone_Deck_Tracker.Enums;
using Hearthstone_Deck_Tracker.Hearthstone;
using Hearthstone_Deck_Tracker.Plugins;
using HSCore;
using HSCore.Entities;
using Newtonsoft.Json;

namespace HSPlugin
{
    public class WebApiPlugin : IPlugin
    {
        WebApiUploader _uploader;
        private Guid _gameId;
        private bool _isGameJustStarted;
        public WebApiPlugin()
        {
            _isGameJustStarted = false;
            _uploader = new WebApiUploader();
        }
        public void OnLoad()
        {

            GameEvents.OnTurnStart.Add(OnTurnStart);
          //  GameEvents.OnOpponentDraw.Add(OnOpponentDraw);
            GameEvents.OnOpponentPlay.Add(OnOpponentPlay);
      //      GameEvents.OnOpponentHandDiscard.Add(OnOpponentHandDiscard);
    //        GameEvents.OnPlayerMulligan.Add(OnPlayerMulligan);
  //          GameEvents.OnPlayerGet.Add(OnPlayerGet);
//            GameEvents.OnPlayerDraw.Add(OnPlayerDraw);
        //    GameEvents.OnOpponentGet.Add(OnOpponentGet);
            GameEvents.OnGameStart.Add(OnGameStart);
            GameEvents.OnGameEnd.Add(OnGameEnd);
          //  GameEvents.OnOpponentHeroPower.Add(OnOpponentHeroPower);
            GameEvents.OnGameWon.Add(OnGameWon);
            GameEvents.OnGameLost.Add(OnGameLost);
        }

        public object OnPlay { get; set; }

        private void OnGameWon()
        {
            var message = new HsGameMessage(HSGameEventTypes.OnGameWon);
            message.Data = _gameId;
            PublishMessage(message);
        }

        private void OnGameLost()
        {
            var message = new HsGameMessage(HSGameEventTypes.OnGameLost);
            message.Data = _gameId;
            PublishMessage(message);
        }

        private void OnOpponentHeroPower()
        {
            var message = new HsGameMessage(HSGameEventTypes.OnOpponentHeroPower);
            message.Data = null;
            PublishMessage(message);
        }

        private void OnPlayerDraw(Card card)
        {
            var message = new HsGameMessage(HSGameEventTypes.OnPlayerDraw);
            message.Data = new { GameId = _gameId, Card = card };
            PublishMessage(message);
        }

        private void OnOpponentGet()
        {
            var message = new HsGameMessage(HSGameEventTypes.OnOpponentGet);
            message.Data = new { GameId = _gameId, CoinTo = "Opponent" };
            PublishMessage(message);
        }

        private void OnPlayerGet(Card card)
        {
            var message = new HsGameMessage(HSGameEventTypes.OnPlayerGet);
            message.Data = new { GameId = _gameId, CoinTo = "Player" };
            PublishMessage(message);
        }

        private void OnPlayerMulligan(Card card)
        {
            var message = new HsGameMessage(HSGameEventTypes.OnPlayerMulligan);
            message.Data = card;
            PublishMessage(message);
        }

        private void OnGameStart()
        {
            _isGameJustStarted = true;
            var game = Core.Game;
            var message = new HsGameMessage(HSGameEventTypes.OnGameStart);
            _gameId = Guid.NewGuid();

            HSGameDto dto = new HSGameDto
            {
                GameId = _gameId.ToString(),
                GameMode = Enum.GetName(typeof(GameMode), game.CurrentGameMode), // get from enum
                Region = Enum.GetName(typeof(Region), game.CurrentRegion), // get from enum
                PlayerName = game.Player.Name,
                PlayerClass = game.Player.Class
                
            };
            
            message.Data = dto;
            PublishMessage(message);
        }

        private void OnGameEnd()
        {
            _isGameJustStarted = false;
            var message = new HsGameMessage(HSGameEventTypes.OnGameEnd);
            message.Data = _gameId;
            PublishMessage(message);
        }

        private void OnOpponentHandDiscard(Card card)
        {
            var message = new HsGameMessage(HSGameEventTypes.OnOpponentPlay);

            message.Data = new
            {
                GameId = _gameId,
                card.Id,
                Name = card.LocalizedName
            };
            PublishMessage(message);
        }

        private void OnOpponentPlay(Card card)
        {

            var message = new HsGameMessage(HSGameEventTypes.OnOpponentPlay);
            message.Data = new
            {
                GameId = _gameId,
                card.Id,
                card.Cost,
                Name = card.LocalizedName
            };
            PublishMessage(message);
        }


        private void OnOpponentDraw()
        {
            var message = new HsGameMessage(HSGameEventTypes.OnOpponentDraw);
            message.Data = "";
            PublishMessage(message);
        }

        private void OnTurnStart(ActivePlayer player)
        {
            var message = new HsGameMessage(HSGameEventTypes.OnTurnStart);
            var game = Core.Game;
            if (_isGameJustStarted)
            {
                var cardsList = game.Player.OpponentCardList.Where(x => x.Name != "The Coint").Select(x => x.Id).ToList();
                cardsList.AddRange(game.Player.PlayerCardList.Select(x => x.Id));
                var resultList = cardsList.Distinct();
                HSGameDto dto = new HSGameDto
                {
                    GameId = _gameId.ToString(),
                    Region = Enum.GetName(typeof (Region), game.CurrentRegion), // get from enum
                    GameMode = Enum.GetName(typeof (GameMode), game.CurrentGameMode), // get from enum
                    OpponentClass = game.Opponent.Class,
                    OpponentName = game.Opponent.Name,
                    OpponentRank = game.CurrentGameStats.RankString,
                    PlayerName = game.Player.Name,
                    PlayerClass = game.Player.Class,
                    PlayerHasCoin = game.Player.HasCoin,
                    PlayerCardsIds = resultList
                };
                message.Data = dto;
                PublishMessage(message);
                _isGameJustStarted = false;
            }
            else
            {
                message.Data = null;
                PublishMessage(message);
            }
        }

        public void PublishMessage(HsGameMessage message)
        {
            var serializedString = JsonConvert.SerializeObject(message);
            _uploader.SendPost(serializedString);
        }

        public byte[] MessageToBytes(string message)
        {
            return Encoding.Default.GetBytes(message);
        }

        public void OnUnload()
        {
        }

        public void OnButtonPress()
        {
        }

        public void OnUpdate()
        {
        }

        public string Name => "Event Producer Plugin";
        public string Description => "Data Transmitter";
        public string ButtonText => "Settings";
        public string Author => "AlexP";
        public Version Version => new Version(0, 99);
        public MenuItem MenuItem
        {
            get { return null; }
        }
    }
}
