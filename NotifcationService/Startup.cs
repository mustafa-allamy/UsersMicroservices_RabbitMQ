using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NotifcationService.Interfaces.Repositories;
using NotifcationService.Interfaces.Services;
using NotifcationService.Repositories;
using NotifcationService.Services;
using RabbitMQService;
using System;
using System.Net.Mail;

namespace NotifcationService
{
    public class Startup
    {
        private readonly IWebHostEnvironment _environment;

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _environment = environment;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var oneSignalConfig = Configuration.GetSection("OneSignal");
            services.Configure<OneSignalConfiguration>(oneSignalConfig);
            services.AddScoped<ISenderService, SenderService>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddDbContext<NotificationContext>(options =>
                    options.UseNpgsql("ConnectionStrings:Local"));
            var serviceClientSettingsConfig = Configuration.GetSection("RabbitMq");
            var serviceClientSettings = serviceClientSettingsConfig.Get<RabbitMqConfiguration>();
            services.Configure<RabbitMqConfiguration>(serviceClientSettingsConfig);
            serviceClientSettings.QueueName = "UserAddedNotification";
            if (serviceClientSettings.Enabled)
            {
                services.AddHostedService(x => new UserAddedNotificationConsumer(serviceClientSettings, x.GetRequiredService<IServiceProvider>()));
            }
            var secoundClient = (RabbitMqConfiguration)serviceClientSettings.Shallowcopy();
            secoundClient.QueueName = "UserUpdatedNotification";
            if (serviceClientSettings.Enabled)
            {
                services.AddHostedService(x => new UserUpdatedNotificationConsumer(secoundClient, x.GetRequiredService<IServiceProvider>()));
            }

            // This code commented uncomment it when u need to send from a real server

            //var client = new SmtpClient
            //{
            //    Credentials = new NetworkCredential(Configuration.GetSection("FluentEmail:UserName").Value,
            //        Configuration.GetSection("FluentEmail:Password").Value),
            //    Host = Configuration.GetSection("FluentEmail:Hostname").Value,
            //    Port = int.Parse(Configuration.GetSection("FluentEmail:Port").Value)
            //};
            var client = new SmtpClient
            {
                EnableSsl = false,
                Host = Configuration.GetSection("FluentEmail:Hostname").Value,
                DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
                PickupDirectoryLocation = _environment.WebRootPath + @"\Emails\"
            };
            services
                .AddFluentEmail(Configuration.GetSection("FluentEmail:Sender").Value)
                .AddRazorRenderer()
                .AddSmtpSender(client);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();

            app.UseRouting();


        }
    }
}
