using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using RabbitMQ.Client;
using Microsoft.Extensions.Configuration;

namespace Publisher_Api.Services
{
    // define interface and service
    public interface IMessageQueueService
    {
        bool Enqueue(string message);
    }

    public class MessageQueueService : IMessageQueueService
    {
        ConnectionFactory _factory;
        IConnection _conn;
        IModel _channel;
        private IConfiguration _configuration;
        public MessageQueueService(IConfiguration configuration)
        {
            _configuration = configuration;
         
            Console.WriteLine("about to connect to rabbit");
            _factory = new ConnectionFactory() { HostName = _configuration["RabbitMq:HostName"] };
            _factory.UserName = _configuration["RabbitMq:UserName"];
            _factory.Password = _configuration["RabbitMq:Password"];
            _factory.Port = AmqpTcpEndpoint.UseDefaultPort;
            _factory.VirtualHost = "/";
            _conn = _factory.CreateConnection();
            _channel = _conn.CreateModel();
            _channel.QueueDeclare(queue: _configuration["RabbitMq:Queue"],
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);
        }
        public bool Enqueue(string messageString)
        {
            var body = Encoding.UTF8.GetBytes(messageString);
            _channel.BasicPublish(exchange: "",
                                routingKey: _configuration["RabbitMq:Queue"],
                                basicProperties: null,
                                body: body);
            return true;
        }
    }
}