using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit;
using MassTransit.Definition;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Play.Catalog.Service.Repositories;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Data;
using Play.Inventory.Service.Entities;
using Play.Inventory.Service.Repositories;
using Play.Inventory.Service.settings;
using Polly;
using Polly.Timeout;

namespace Play.Catalog.Service.Extension
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IRepository<InventoryItem>, Repository<InventoryItem>>();
            services.AddScoped<IRepository<CatalogItem>, Repository<CatalogItem>>();

            services.AddDbContext<DataContext>(option =>
           {
               option.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
           });

            services.AddMassTransit(x =>
        {
            x.AddConsumers(Assembly.GetEntryAssembly());
            x.UsingRabbitMq((context, configurator) =>
            {
                var rabbitMQSettings = configuration.GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>();
                configurator.Host(rabbitMQSettings.Host);
                configurator.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(false));
                configurator.UseMessageRetry(retryConfigurator =>
                {
                    retryConfigurator.Interval(3, TimeSpan.FromSeconds(5));

                });

            });
        });
            services.AddMassTransitHostedService();

            //This is synchronous communication
            Random jitterer = new Random();

            services.AddHttpClient<CatalogClients>(option =>
            {
                option.BaseAddress = new Uri("https://localhost:5001");

            })
            .AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>().WaitAndRetryAsync(
                5,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(jitterer.Next(0, 1000)),
                onRetry: (outcome, timespan, retryAttempt) =>
                {
                    var serviceProvider = services.BuildServiceProvider();
                    serviceProvider.GetService<ILogger<CatalogClients>>()?
                                    .LogWarning($"Delaying for {timespan.TotalSeconds} seconds, then making retry {retryAttempt}");
                }
            ))
            .AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>().CircuitBreakerAsync(
                3,
                TimeSpan.FromSeconds(15),
                onBreak: (outcome, timespan) =>
                {
                    var serviceProvider = services.BuildServiceProvider();
                    serviceProvider.GetService<ILogger<CatalogClients>>()?
                                    .LogWarning($"Opening the circuit for {timespan.TotalSeconds} seconds...");

                },
                onReset: () =>
                {
                    var serviceProvider = services.BuildServiceProvider();
                    serviceProvider.GetService<ILogger<CatalogClients>>()?
                                    .LogWarning($"closing the circuit for...");
                }
            ))
            .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));





            return services;
        }

    }
}