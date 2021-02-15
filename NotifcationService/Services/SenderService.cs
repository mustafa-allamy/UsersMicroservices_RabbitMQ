using FluentEmail.Core;
using Microsoft.Extensions.Options;
using NotifcationService.Interfaces.Services;
using OneSignal.RestAPIv3.Client;
using OneSignal.RestAPIv3.Client.Resources;
using OneSignal.RestAPIv3.Client.Resources.Notifications;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NotifcationService.Services
{
    public class SenderService : ISenderService
    {
        private readonly IFluentEmail _fluentEmail;
        private readonly string _apiKey;
        private readonly string _appId;
        public SenderService(IOptions<OneSignalConfiguration> oneSignalOptions, IFluentEmail fluentEmail)
        {
            _fluentEmail = fluentEmail;
            _apiKey = oneSignalOptions.Value.APIKey;
            _appId = oneSignalOptions.Value.AppId;
        }


        public async Task SendNotification(string header, string content, List<string> externalUserIds)
        {
            var client = new OneSignalClient(_apiKey); // Use your Api Key

            var options = new NotificationCreateOptions
            {
                AppId = new Guid(_appId),   // Use your AppId
                IncludeExternalUserIds = externalUserIds
            };
            options.Headings.Add(LanguageCodes.English, header);
            options.Contents.Add(LanguageCodes.English, content);

            await client.Notifications.CreateAsync(options);
        }
        public async Task SendNotification(string header, string content, string includedSegments)
        {
            var client = new OneSignalClient(_apiKey); // Use your Api Key

            var options = new NotificationCreateOptions
            {
                AppId = new Guid(_appId),   // Use your AppId
                IncludedSegments = new string[] { includedSegments }

            };
            options.Headings.Add(LanguageCodes.English, header);
            options.Contents.Add(LanguageCodes.English, content);

            await client.Notifications.CreateAsync(options);
        }

        public async Task SendEmail(string to, string name, string subject, string body)
        {

            var template = @$"Dear @Model.Name, {body}";
            await _fluentEmail
                .To("test@test.test")
                .Subject("test email subject")
                .UsingTemplate(template, new { Name = name }).SendAsync();

        }

    }
}