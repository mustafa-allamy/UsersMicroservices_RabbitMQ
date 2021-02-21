using FluentEmail.Core;
using Microsoft.Extensions.Options;
using Models.Enums;
using Models.Models;
using NotifcationService.Interfaces.Repositories;
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
        private readonly INotificationRepository _notificationRepository;
        private readonly string _apiKey;
        private readonly string _appId;
        public SenderService(IOptions<OneSignalConfiguration> oneSignalOptions, IFluentEmail fluentEmail, INotificationRepository notificationRepository)
        {
            _fluentEmail = fluentEmail;
            _notificationRepository = notificationRepository;
            _apiKey = oneSignalOptions.Value.APIKey;
            _appId = oneSignalOptions.Value.AppId;
        }


        public async Task SendNotification(string header, string content, List<Guid> externalUserIds)
        {
            var client = new OneSignalClient(_apiKey); // Use your Api Key

            var options = new NotificationCreateOptions
            {
                AppId = new Guid(_appId),   // Use your AppId
                IncludeExternalUserIds = externalUserIds.ConvertAll(x => x.ToString())
            };
            options.Headings.Add(LanguageCodes.English, header);
            options.Contents.Add(LanguageCodes.English, content);
            await client.Notifications.CreateAsync(options);
            await SaveNotification(content, header, null, NotificationType.NormalNotification, true, null, externalUserIds);


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
            await SaveNotification(content, header, null, NotificationType.NormalNotification, true, includedSegments, new List<Guid>());

        }

        public async Task SendEmail(string to, string name, string subject, string body)
        {

            var template = @$"Dear @Model.Name, {body}";
            var res = await _fluentEmail
                 .To("test@test.test")
                 .Subject("test email subject")
                 .UsingTemplate(template, new { Name = name }).SendAsync();

            await SaveNotification(body, subject, to, NotificationType.Email, res.Successful, null, new List<Guid>());

        }

        public async Task SaveNotification(string body, string head, string email, NotificationType type, bool status, string includedSegment, List<Guid> usersIds)
        {
            await _notificationRepository.Insert(new Notification()
            {
                Content = body,
                Head = head,
                Email = email,
                NotificationType = NotificationType.Email,
                IsDelevried = status,
                IncludedSegment = includedSegment,
                IncludedUsersIds = usersIds
            });
            await _notificationRepository.Commit();
        }

    }
}