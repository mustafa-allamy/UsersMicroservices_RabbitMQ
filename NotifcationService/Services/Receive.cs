using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Models.Models;
using Newtonsoft.Json;
using NotifcationService.Interfaces.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQService;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NotifcationService.Services
{
    public class Receive : BackgroundService
    {
        private readonly ISenderService _senderService;
        private IModel _channel;
        private IConnection _connection;
        private readonly string _hostname;
        private readonly string _queueName;
        private readonly string _username;
        private readonly string _password;

        public Receive(IOptions<RabbitMqConfiguration> rabbitMqOptions, ISenderService senderService)
        {
            _senderService = senderService;
            _hostname = rabbitMqOptions.Value.Hostname;
            _username = rabbitMqOptions.Value.UserName;
            _password = rabbitMqOptions.Value.Password;
            _queueName = rabbitMqOptions.Value.QueueName;

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
                var updateCustomerFullNameModel = JsonConvert.DeserializeObject<User>(content);

                await HandleMessage(updateCustomerFullNameModel);

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(_queueName, false, consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(User user)
        {
            await _senderService.SendNotification(user.FullName, "Password Has Changed",
                new List<string>() { user.Id.ToString() });
            await _senderService.SendEmail(user.Email, user.FullName, "Account Information Update",
                "Your account info have been updated");
        }



        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}