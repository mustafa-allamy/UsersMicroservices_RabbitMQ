using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Models.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQService;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AuthService.Services
{
    public class Receive : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private IModel _channel;
        private IConnection _connection;
        private readonly string _hostname;
        private readonly string _queueName;
        private readonly string _username;
        private readonly string _password;

        public Receive(IOptions<RabbitMqConfiguration> rabbitMqOptions, IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
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
                var user = JsonConvert.DeserializeObject<User>(content);

                await HandleMessage(user);

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(_queueName, false, consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(User user)
        {
            var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AuthContext>();
            var oldUser = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == user.Id);
            if (oldUser != null)
            {
                oldUser = user;
                db.Users.Update(oldUser);
            }
            else
            {
                await db.Users.AddAsync(user);
            }

            await db.SaveChangesAsync();
            scope.Dispose();
            await db.DisposeAsync();
        }



        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();

            base.Dispose();
        }
    }
}