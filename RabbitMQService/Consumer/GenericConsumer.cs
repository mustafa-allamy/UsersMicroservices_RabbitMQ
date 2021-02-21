using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQService.Consumer
{
    public class GenericConsumer : BackgroundService
    {
        public IServiceProvider _serviceProvider;
        private IModel _channel;
        private IConnection _connection;
        private readonly string _hostname;
        public string _queueName;
        private readonly string _username;
        private readonly string _password;

        public GenericConsumer(RabbitMqConfiguration rabbitMqOptions, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _hostname = rabbitMqOptions.Hostname;
            _username = rabbitMqOptions.UserName;
            _password = rabbitMqOptions.Password;
            _queueName = rabbitMqOptions.QueueName;
            InitializeRabbitMqListener();
        }

        private void InitializeRabbitMqListener()
        {
            var factory = new ConnectionFactory
            {
                HostName = _hostname,
                UserName = _username,
                Password = _password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());

                await HandleMessage(content);

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(_queueName, false, consumer);

            return Task.CompletedTask;
        }

        public virtual async Task HandleMessage(string message)
        {

        }


    }
}