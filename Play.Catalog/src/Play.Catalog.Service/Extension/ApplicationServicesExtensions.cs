using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit;
using MassTransit.Definition;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Play.Catalog.Service.Data;
using Play.Catalog.Service.Entities;
using Play.Catalog.Service.Repositories;
using Play.Catalog.Service.Settings;

namespace Play.Catalog.Service.Extension
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IRepository<Item>, Repository<Item>>();

            services.AddDbContext<DataContext>(option =>
           {
               option.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
           });


            services.AddMassTransit(x =>
            {
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


            return services;
        }

    }
}