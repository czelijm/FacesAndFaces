using GreenPipes;
using MassTransit;
using Messaging.InterfacesConstants.Constants;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NotificationService.Consumer;
using NotificationService.Services;
using System;
using System.Threading.Tasks;

namespace NotificationService
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await host.RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            var hostBuilder = Host.CreateDefaultBuilder(args).ConfigureServices((hostContext,services)=> 
            {
                //services.AddMassTransit(c=> 
                //{
                //    c.AddConsumer<OrderProcessedEventConsumer>();

                //});

                //services.AddSingleton(provider=> Bus.Factory.CreateUsingRabbitMq(cfg=> 
                //{
                //    cfg.Host(RabbitMqMassTransitConstants.HostName,"/", h=> { });
                //    cfg.ReceiveEndpoint(RabbitMqMassTransitConstants.NotificationServiceQueue,e=> 
                //    {
                //        e.PrefetchCount=RabbitMqMassTransitConstants.PerfectchCount;
                //        e.UseMessageRetry(x=>x.Interval(
                //            RabbitMqMassTransitConstants.RetryNumber,
                //            TimeSpan.FromSeconds(RabbitMqMassTransitConstants.ItervalWaitTimeInSeconds)));
                //        e.Consumer<OrderProcessedEventConsumer>(provider);
                //    });
                //    cfg.ConfigureEndpoints(provider);
                //}));
                services.AddMassTransit(c =>
                {
                    c.AddConsumer<OrderProcessedEventConsumer>();

                    c.UsingRabbitMq((contex, cfg) =>
                    {
                        cfg.Host(
                              RabbitMqMassTransitConstants.HostName,
                             "/",
                             h => { }
                         );
                        cfg.ReceiveEndpoint(
                             RabbitMqMassTransitConstants.NotificationServiceQueue,
                             e =>
                             {
                                 e.PrefetchCount = 16;//nuber of concurrent messages
                                e.UseMessageRetry(
                                     x => x.Interval(
                                         RabbitMqMassTransitConstants.RetryNumber,
                                         TimeSpan.FromSeconds(RabbitMqMassTransitConstants.ItervalWaitTimeInSeconds)
                                     )); //retry policy of 10s, try 2 times 
                                e.Consumer<OrderProcessedEventConsumer>(contex);
                             });
                        cfg.ConfigureEndpoints(contex);
                    });//resiliency setup; fetching data over the wire
                });

                services.AddSingleton<IHostedService, BusService>();

            });

            return hostBuilder;
        }
    }
}
