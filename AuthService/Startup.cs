using AuthService.Interfaces.Repositories;
using AuthService.Interfaces.Services;
using AuthService.Repositories;
using AuthService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RabbitMQService;
using System;
using System.Text;

namespace AuthService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AuthContext>(options =>
                options.UseNpgsql("ConnectionStrings:Local"), ServiceLifetime.Scoped);
            services.AddCors(o => o.AddPolicy("TestEnv",
                builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); }));
            var serviceClientSettingsConfig = Configuration.GetSection("RabbitMq");
            var serviceClientSettings = serviceClientSettingsConfig.Get<RabbitMqConfiguration>();

            serviceClientSettings.QueueName = "AddUser";

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();


            if (serviceClientSettings.Enabled)
            {
                services.AddHostedService(x => new AddUserConsumer(serviceClientSettings, x.GetRequiredService<IServiceProvider>()));
            }
            var secoundClient = (RabbitMqConfiguration)serviceClientSettings.Shallowcopy();
            secoundClient.QueueName = "UpdateUser";
            if (serviceClientSettings.Enabled)
            {
                services.AddHostedService(x => new UpdateUserConsumer(secoundClient, x.GetRequiredService<IServiceProvider>()));
            }
            services.AddAuthentication(defaultScheme: JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(configureOptions: jwtBearerOptions =>
                {
                    jwtBearerOptions.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateActor = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = "Issuer",
                            ValidAudience = "Audience",
                            IssuerSigningKey =
                                new SymmetricSecurityKey(key: Encoding
                                    .UTF8
                                    .GetBytes(s: "dmiWqigAEvWmCq5TgJLhuHvByNY5PCnb"))
                        };
                });
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AuthService", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthService v1"));
            }
            var scope = app.ApplicationServices.CreateScope();
            var service = scope.ServiceProvider.GetService<AuthContext>();
            app.UseHttpsRedirection();
            app.UseCors("TestEnv");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
