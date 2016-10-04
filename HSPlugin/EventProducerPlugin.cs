using System;
using System.Collections.Generic;
using System.Text;
using Hearthstone_Deck_Tracker.Plugins;
using System.Windows.Controls;
using HearthDb;
using Hearthstone_Deck_Tracker.API;
using Hearthstone_Deck_Tracker.Enums;
using HSCore;
using HSCore.Entities;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Card = Hearthstone_Deck_Tracker.Hearthstone.Card;

namespace HSPlugin
{
    public class EventProducer : IPlugin
    {
        private const string HostName = "localhost";
        private const string UserName = "guest";
        private const string Password = "guest";
        private const string ExchangeName = "HsAnalyzer";
        private const string QueueName = "HsEventsQueue";

        ConnectionFactory _factory;
        private IConnection _connection;
        private IModel _model;
        private IBasicProperties _properties;
        private Guid _gameId;

        public EventProducer()
        {
            _factory = new ConnectionFactory
            {

                HostName = HostName,
                UserName = UserName,
                Password = Password
            };
            _connection = _factory.CreateConnection();
            _model = _connection.CreateModel();
            _model.ExchangeDeclare(ExchangeName, ExchangeType.Direct, true);

            _model.QueueDeclare(QueueName, true, false, false, null);
            _model.QueueBind(QueueName, ExchangeName, "");
            _properties = _model.CreateBasicProperties();
            _properties.Persistent = true;
            

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
            var message = new HsGameMessage(HSGameEventTypes.OnOpponentGet);
            message.Data = new { GameId = _gameId, Card = card };
            PublishMessage(message);
        }

        private void OnOpponentGet()
        {
            var message = new HsGameMessage(HSGameEventTypes.OnOpponentGet);
            message.Data = new {GameId = _gameId, CoinTo="Opponent"};
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
            var message = new HsGameMessage(HSGameEventTypes.OnGameStart);
            _gameId = Guid.NewGuid();
            message.Data = _gameId;
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
            message.Data = player;
            PublishMessage(message);
        }

        public void PublishMessage(HsGameMessage message)
        {
            var serializedString = JsonConvert.SerializeObject(message);
            var byteArray = MessageToBytes(serializedString);
            _model.BasicPublish(ExchangeName, "", _properties, byteArray);
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
        public Version Version =>new Version(0,99);
        public MenuItem MenuItem
        {
            get { return null; }
        }
    }
}
