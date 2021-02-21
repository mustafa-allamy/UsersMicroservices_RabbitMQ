using AuthService.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Models.Models;
using Newtonsoft.Json;
using RabbitMQService;
using RabbitMQService.Consumer;
using System;
using System.Threading.Tasks;

namespace AuthService.Services
{
    public class UpdateUserConsumer : GenericConsumer
    {


        public override async Task HandleMessage(string message)
        {
            var scope = _serviceProvider.CreateScope();

            var handler = scope.ServiceProvider.GetRequiredService<IUserService>();

            var user = JsonConvert.DeserializeObject<User>(message);
            if (user != null)
            {
                await handler.UpdateUser(user);
            }
        }

        public UpdateUserConsumer(RabbitMqConfiguration rabbitMqOptions, IServiceProvider serviceProvider) : base(rabbitMqOptions, serviceProvider)
        {

        }


    }
}