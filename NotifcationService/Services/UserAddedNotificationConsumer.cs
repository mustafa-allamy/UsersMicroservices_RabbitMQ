using Microsoft.Extensions.DependencyInjection;
using Models.Dtos;
using Newtonsoft.Json;
using NotifcationService.Interfaces.Services;
using RabbitMQService;
using RabbitMQService.Consumer;
using System;
using System.Threading.Tasks;

namespace NotifcationService.Services
{
    public class UserAddedNotificationConsumer : GenericConsumer
    {


        public override async Task HandleMessage(string message)
        {
            var scope = _serviceProvider.CreateScope();

            var handler = scope.ServiceProvider.GetRequiredService<ISenderService>();

            var user = JsonConvert.DeserializeObject<UserAddedNotificationDto>(message);
            if (user != null)
                await handler.SendEmail(user.Email, user.Name, "Account Created", "Your new account has been created");
        }

        public UserAddedNotificationConsumer(RabbitMqConfiguration rabbitMqOptions, IServiceProvider serviceProvider) : base(rabbitMqOptions, serviceProvider)
        {

        }
    }
}