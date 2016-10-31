using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using HSCore;
using HSCore.Entities;
using Microsoft.AspNet.SignalR;
using MongoRepository;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.MessagePatterns;
using IConnection = RabbitMQ.Client.IConnection;

namespace HSGameAnalyzer
{
    public class EventConsumer
    {
        private const string HostName = "localhost";
        private const string UserName = "guest";
        private const string Password = "guest";
        private const string ExchangeName = "HsAnalyzer";
        private const string QueueName = "HsEventsQueue";
        Subscription _subscription;
        private readonly MongoRepository<HSGame> _gameRepository;
        private readonly MongoRepository<HSDeck> _deckRepository;
        ConnectionFactory _factory;
        private IConnection _connection;
        private IModel _model;
        private IBasicProperties _properties;

        public EventConsumer()
        {
            _factory = new ConnectionFactory
            {

                HostName = HostName,
                UserName = UserName,
                Password = Password
            };
            _connection = _factory.CreateConnection();
            _model = _connection.CreateModel();
            _model.QueueDeclare(QueueName, true, false, false, null);
            _model.QueueBind(QueueName, ExchangeName, "");
            _model.BasicQos(0, 1, false);
            _subscription = new Subscription(_model, QueueName, false);
            _gameRepository = new MongoRepository<HSGame>();
            _deckRepository= new MongoRepository<HSDeck>();
        }

        public void ConsumeDat()
        {
            var consumer = new EventingBasicConsumer(_model);
            consumer.Received += (model, ea) =>
            {
                var str = Encoding.Default.GetString(ea.Body);
                var message = JsonConvert.DeserializeObject<HsGameMessage>(str);
                //Handle Message
                DisplayMessage(message);
                //Console.WriteLine(message);
            };
            _model.BasicConsume(queue: QueueName,
                                           noAck: true,
                                           consumer: consumer);
        }

        private delegate void ConsumeDelegate();

        private void DisplayMessage(HsGameMessage message)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<HSGameHub>();
            HSGameDto gameDto;
            switch (message.EventType)
            {
                case HSGameEventTypes.OnGameStart:
                    CreateGame(message);
                    hubContext.Clients.All.sendMessage(message);
                    break;
                case HSGameEventTypes.OnTurnStart:
                    try
                    {
                        UpdateGameInDb(message);
                    }
                    catch (Exception ex)
                    {
                        message.Data = null;
                    }
                    hubContext.Clients.All.sendMessage(message);
                    break;
                case HSGameEventTypes.OnPlayerGet:
                    hubContext.Clients.All.sendMessage(message);
                    break;
                case HSGameEventTypes.OnOpponentGet:
                    hubContext.Clients.All.sendMessage(message);
                    break;
                case HSGameEventTypes.OnOpponentDraw:
                    hubContext.Clients.All.sendMessage(message);
                    break;
                case HSGameEventTypes.OnOpponentPlay:
                    hubContext.Clients.All.sendMessage(message);
                    break;
                case HSGameEventTypes.OnPlayerDraw:
                    hubContext.Clients.All.sendMessage(message);
                    break;
                case HSGameEventTypes.OnGameLost:
                    hubContext.Clients.All.sendMessage(message);
                    break;
                case HSGameEventTypes.OnGameWon:
                    hubContext.Clients.All.sendMessage(message);
                    break;
            }
        }

        private void UpdateGameInDb(HsGameMessage message)
        {
            HSGameDto gameDto = message.Data.ToObject<HSGameDto>();
            var gameEntity = _gameRepository.FirstOrDefault(x => x.GameId == gameDto.GameId);
            gameEntity.Date = DateTime.Now;
            gameEntity.Region = gameDto.Region;
            gameEntity.GameMode = gameDto.GameMode;
            gameEntity.OpponentClass = gameDto.OpponentClass;
            gameEntity.OpponentName = gameDto.OpponentName;
            gameEntity.OpponentRank = gameDto.OpponentRank;
            gameEntity.PlayerClass = gameDto.PlayerClass;
            gameEntity.PlayerName = gameDto.PlayerName;
            gameEntity.PlayerRank = gameDto.PlayerRank;
            gameEntity.OpponentDeckId = gameDto.OpponentDeckId;
            gameEntity.OpponentDeckType = gameDto.OpponentDeckType;
            gameEntity.PlayerDeckId = GetPlayerDeckArchetype(message).Item2;
            gameEntity.PlayerDeckType = GetPlayerDeckArchetype(message).Item1;
            gameEntity.PlayerHasCoin = gameDto.PlayerHasCoin;

            _gameRepository.Update(gameEntity);
        }

        private Tuple<string,string> GetPlayerDeckArchetype(HsGameMessage message)
        {
            HSGameDto gameDto = message.Data.ToObject<HSGameDto>();

            var deckList =_deckRepository.Where(x => x.Class == gameDto.PlayerClass.ToUpper());
            int count = 0;
            string deckArchetype = string.Empty;
            string deckId = "";
            foreach (var deck in deckList)
            {
                var ids = deck.Cards.Select(x => x.cardId).ToList();
                var result = gameDto.PlayerCardsIds.Intersect(ids).ToList();
                if (result.Count() > count)
                {
                    count = result.Count();
                    deckArchetype = deck.Type;
                    deckId = deck.Id;
                }

            }
            
            return new Tuple<string, string>(deckArchetype, deckId);
        }

        private void CreateGame(HsGameMessage message)
        {
            var gameDto = message.Data.ToObject<HSGameDto>();
            var gameEntity = Mapper.Map<HSGame>(gameDto);
            _gameRepository.Add(gameEntity);
        }
    }
}
