using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using HSCore.Entities;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace HsGameWebApi
{
    public class BusPublisher : IDisposable
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
        private bool _disposed;

        public BusPublisher()
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
            
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Clear all property values that maybe have been set
                    // when the class was instantiated
                    _model.Close();
                    _connection.Close();
                }

                // Indicate that the instance has been disposed.
                _disposed = true;
            }
        }
    }
}