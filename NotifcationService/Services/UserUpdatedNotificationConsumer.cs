using Microsoft.Extensions.DependencyInjection;
using Models.Dtos;
using Newtonsoft.Json;
using NotifcationService.Interfaces.Services;
using RabbitMQService;
using RabbitMQService.Consumer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NotifcationService.Services
{
    public class UserUpdatedNotificationConsumer : GenericConsumer
    {


        public override async Task HandleMessage(string message)
        {
            var scope = _serviceProvider.CreateScope();

            var handler = scope.ServiceProvider.GetRequiredService<ISenderService>();
            var user = JsonConvert.DeserializeObject<UserUpdatedNotificationDto>(message);
            if (user != null)
            {
                await handler.SendEmail(user.Email, user.Name, "Account info updated", "Your new account information has been updated");
                await handler.SendNotification("Account info updated", "Your new account information has been updated",
                    new List<Guid>() { user.UserId });
            }
        }

        public UserUpdatedNotificationConsumer(RabbitMqConfiguration rabbitMqOptions, IServiceProvider serviceProvider) : base(rabbitMqOptions, serviceProvider)
        {

        }
    }
}