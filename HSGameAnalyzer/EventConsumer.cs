using System;
using System.Collections.Generic;
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
        ConnectionFactory _factory;
        private IConnection _connection;
        private IModel _model;
        private IBasicProperties _properties;

        private readonly MongoRepository<HSGame> _gameRepository;
        public EventConsumer()
        {
            MessageHandler handler = new MessageHandler();
            _factory = new ConnectionFactory
            {

                HostName = HostName,
                UserName = UserName,
                Password = Password
            };
            _connection = _factory.CreateConnection();
            _model = _connection.CreateModel();
            _model.QueueDeclare(QueueName, false, false, false, null);
            _model.QueueBind(QueueName, ExchangeName, "");
            _model.BasicQos(0, 1, false);
            _subscription = new Subscription(_model, QueueName, false);

            /*var consumer = new ConsumeDelegate(Poll);
            consumer.Invoke();*/
            _gameRepository = new MongoRepository<HSGame>();


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
            switch (message.EventType)
            {
                case HSGameEventTypes.OnGameStart:
                    string gameId = message.Data.ToString();
                    var game = new HSGameDto()
                    {
                        EventType = message.EventType,
                        GameId = gameId,
                    };
                    message.Data = game;
                    hubContext.Clients.All.sendMessage(message);
                    _gameRepository.Add(Mapper.Map<HSGame>(game));
                    break;
                case HSGameEventTypes.OnTurnStart:
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
                    /* case HSGameEventTypes.OnOpponentHandDiscard:
                         hubContext.Clients.All.sendMessage(message);
                         break;
                     case HSGameEventTypes.OnGameEnd:
                         hubContext.Clients.All.sendMessage(message);
                         break;*/
            }
        }
    }
}
