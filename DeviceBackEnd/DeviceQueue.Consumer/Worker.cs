using DeviceQueue.Consumer.Model;
using DeviceQueue.Consumer.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceQueue.Consumer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        
        private IConnection _connection;
        private IModel _channel;
        ConnectionFactory _factory;
        private IConfiguration _configuration;

        private readonly IDeviceService _deviceService;

        public Worker(ILogger<Worker> logger, IConfiguration configuration, IDeviceService deviceService)
        {
            _logger = logger;
            _configuration = configuration;

            _deviceService = deviceService;

            InitializeRabbitMqListener();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                //var t = JObject.Parse(content);
                var deviceModel = JsonConvert.DeserializeObject<Device>(content);

                HandleMessage(deviceModel);

                _channel.BasicAck(ea.DeliveryTag, false);
            };
            consumer.Shutdown += OnConsumerShutdown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

            _channel.BasicConsume(queue: _configuration["RabbitMq:Queue"], autoAck: false, consumer: consumer);

            return Task.CompletedTask;
        }
        private void InitializeRabbitMqListener()
        {
            _factory = new ConnectionFactory() { HostName = _configuration["RabbitMq:HostName"] };
            _factory.UserName = _configuration["RabbitMq:UserName"];
            _factory.Password = _configuration["RabbitMq:Password"];
            _factory.Port = AmqpTcpEndpoint.UseDefaultPort;
            _factory.VirtualHost = "/";
            _connection = _factory.CreateConnection();
            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: _configuration["RabbitMq:Queue"],
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

        }
        private async void HandleMessage(Device device)
        {
            //Update in db / storage with device status "Assigned"
            if (device.status.Equals("Assign"))
            {
                device.status = "Assigned";
            }

            await _deviceService.Update(device);
        }

        private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e)
        {
        }

        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e)
        {
        }

        private void OnConsumerRegistered(object sender, ConsumerEventArgs e)
        {
        }

        private void OnConsumerShutdown(object sender, ShutdownEventArgs e)
        {
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }

}
