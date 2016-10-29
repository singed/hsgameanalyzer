using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        WebApiUploader uploader;
        private Guid _gameId;
        public WebApiPlugin()
        {
            Trace.WriteLine("Plageroniz loading");
            uploader = new WebApiUploader();
        }
        public void OnLoad()
        {
            GameEvents.OnTurnStart.Add(OnTurnStart);
            GameEvents.OnOpponentDraw.Add(OnOpponentDraw);
            GameEvents.OnOpponentPlay.Add(OnOpponentPlay);
            GameEvents.OnOpponentHandDiscard.Add(OnOpponentHandDiscard);
            GameEvents.OnPlayerMulligan.Add(OnPlayerMulligan);
            GameEvents.OnPlayerGet.Add(OnPlayerGet);
            GameEvents.OnPlayerDraw.Add(OnPlayerDraw);
            GameEvents.OnOpponentGet.Add(OnOpponentGet);
            GameEvents.OnGameStart.Add(OnGameStart);
            GameEvents.OnGameEnd.Add(OnGameEnd);
            GameEvents.OnOpponentHeroPower.Add(OnOpponentHeroPower);
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
            var game = Core.Game;
            var message = new HsGameMessage(HSGameEventTypes.OnGameStart);
            _gameId = Guid.NewGuid();
            message.Data = new
            {
                GameId = _gameId,
                GameMode = game.CurrentGameMode, // get from enum
                Region = game.CurrentRegion, // get from enum
                OpponentName = game.Opponent.Name,
                OpponentClass = game.Opponent.Class
            };
            PublishMessage(message);
        }

        private void OnGameEnd()
        {
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
            message.Data = new
            {
                GameId = _gameId,
                GameMode = game.CurrentGameMode, // get from enum
                Region = game.CurrentRegion, // get from enum
                OpponentName = game.Opponent.Name,
                OpponentClass = game.Opponent.Class
            }; ;
            PublishMessage(message);
        }

        public void PublishMessage(HsGameMessage message)
        {
            var serializedString = JsonConvert.SerializeObject(message);
            uploader.Send(serializedString);
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
