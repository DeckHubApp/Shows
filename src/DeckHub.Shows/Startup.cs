using System;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DeckHub.Shows.Data;
using DeckHub.Shows.Messaging;
using DeckHub.Shows.Services;
using StackExchange.Redis;

namespace DeckHub.Shows
{
    [PublicAPI]
    public class Startup
    {
        private readonly IHostingEnvironment _env;
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            _env = env;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IIdentityPaths, IdentityPaths>();

            ConfigureAuth(services);

            var connectionMultiplexer = ConfigureRedis(services);

            ConfigureDataProtection(services, connectionMultiplexer);

            services.AddShowData(b =>
            {
                b.UseNpgsql(Configuration.GetConnectionString("Shows"));
            });

            services.Configure<MessagingOptions>(Configuration.GetSection("Messaging"));
            services.AddSingleton<IShowSlideQueueClient, ShowSlideQueueClient>();
            services.AddSingleton<IShowStartQueueClient, ShowStartQueueClient>();
            services.AddSingleton<IHostedService, ShowStartProcessor>();
            services.AddSingleton<IHostedService, ShowSlideProcessor>();
            services.AddSingleton<RedisPublisher>();

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddMetrics();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment() || string.Equals(Configuration["Runtime:DeveloperExceptionPage"], "true", StringComparison.OrdinalIgnoreCase))
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            var pathBase = Configuration["Runtime:PathBase"];
            if (!string.IsNullOrEmpty(pathBase))
            {
                app.UsePathBase(pathBase);
            }

            app.UseStaticFiles();

            if (env.IsDevelopment())
            {
                app.UseMiddleware<BypassAuthMiddleware>();
            }
            else
            {
                app.UseAuthentication();
            }

            app.UseMvc();
        }

        
        private void ConfigureAuth(IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();
        }

        private ConnectionMultiplexer ConfigureRedis(IServiceCollection services)
        {
            var redisHost = Configuration.GetSection("Redis").GetValue<string>("Host");
            if (!string.IsNullOrWhiteSpace(redisHost))
            {
                var redisPort = Configuration.GetSection("Redis").GetValue<int>("Port");
                if (redisPort == 0)
                {
                    redisPort = 6379;
                }

                var connectionMultiplexer = ConnectionMultiplexer.Connect($"{redisHost}:{redisPort}");
                services.AddSingleton(connectionMultiplexer);
                return connectionMultiplexer;
            }

            return null;
        }

        private void ConfigureDataProtection(IServiceCollection services, ConnectionMultiplexer connectionMultiplexer)
        {
            if (!_env.IsDevelopment())
            {
                var dpBuilder = services.AddDataProtection().SetApplicationName("deckhub");

                if (connectionMultiplexer != null)
                {
                    dpBuilder.PersistKeysToRedis(connectionMultiplexer, "DataProtection:Keys");
                }
            }
            else
            {
                services.AddDataProtection()
                    .DisableAutomaticKeyGeneration()
                    .SetApplicationName("deckhub");
            }
        }

    }
}
