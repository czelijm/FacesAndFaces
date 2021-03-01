using GreenPipes;
using MassTransit;
using Messaging.InterfacesConstants.Constants;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using OrdersApi.Messages.Consumers;
using OrdersApi.Persistence;
using OrdersApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersApi
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
            services.AddDbContext<OrdersContext>(options=>options.UseSqlServer
            (
                Configuration.GetConnectionString("OrderContextConnection")        
            ));

            services.AddMassTransit(c =>
               {
                   //there will be 2 consumers
                   c.AddConsumer<RegisterOrderCommandConsumer>();
                   c.AddConsumer<OrderDispatchedEventConsumer>();

                   c.UsingRabbitMq((contex,cfg)=> 
                   {
                       cfg.Host(
                             RabbitMqMassTransitConstants.HostName,
                            "/",
                            h => { }
                        );
                       cfg.ReceiveEndpoint(
                            RabbitMqMassTransitConstants.RegisterOrderCommandQueue,
                            e =>
                            {
                                e.PrefetchCount = 16;//nuber of concurrent messages
                                e.UseMessageRetry(
                                    x => x.Interval(
                                        RabbitMqMassTransitConstants.RetryNumber,
                                        TimeSpan.FromSeconds(RabbitMqMassTransitConstants.ItervalWaitTimeInSeconds)
                                    )); //retry policy of 10s, try 2 times 
                                e.Consumer<RegisterOrderCommandConsumer>(contex);
                            });
                       //Another endpoint for another queue
                       cfg.ReceiveEndpoint(
                            RabbitMqMassTransitConstants.OrderDispatchedServiceQueue,
                            e =>
                            {
                                e.PrefetchCount = 16;//nuber of concurrent messages
                                e.UseMessageRetry(
                                    x => x.Interval(
                                        RabbitMqMassTransitConstants.RetryNumber,
                                        TimeSpan.FromSeconds(RabbitMqMassTransitConstants.ItervalWaitTimeInSeconds)
                                    )); //retry policy of 10s, try 2 times 
                                e.Consumer<OrderDispatchedEventConsumer>(contex);
                            });

                       cfg.ConfigureEndpoints(contex);
                   });//resiliency setup; fetching data over the wire
            });

            ////Old Way
            //services.AddMassTransit( c=> 
            //    {
            //        c.AddConsumer<RegisterOrderCommandConsumer>();
            //    }
            //);

            //services.AddSingleton(provider=>Bus.Factory.CreateUsingRabbitMq
            //    (cfg=> 
            //        {
            //            cfg.Host(
            //                 RabbitMqMassTransitConstants.HostName,
            //                "/",
            //                h => { }
            //            );//blank for default parameters
            //            cfg.ReceiveEndpoint(
            //                RabbitMqMassTransitConstants.RegisterOrderCommandQueue,
            //                e=> 
            //                {
            //                    e.PrefetchCount = 16;//nuber of concurrent messages
            //                    e.UseMessageRetry(
            //                        x=>x.Interval(
            //                            RabbitMqMassTransitConstants.RetryNumber,
            //                            TimeSpan.FromSeconds(RabbitMqMassTransitConstants.ItervalWaitTimeInSeconds)
            //                        )

            //                    ); //retry policy of 10s, try 2 times
            //                    e.Consumer<RegisterOrderCommandConsumer>(provider);
            //                }//resiliency setup; fetching data over the wire
            //            );

            //            cfg.ConfigureEndpoints(provider);
            //        }
            //    )
            //);

            
            //register our sevice in dependency injection
            services.AddSingleton<IHostedService, BusService>();

            services.AddTransient<IOrderRepository, OrderRepository>();

            services.AddHttpClient();

            //for order api endpoint to be called from other servers, we have to add cross origin resource policy
            services.AddCors(options=> 
            {
                //we allow any request type, credentials type, header type to be able to reach API endpoints
                options.AddPolicy("CorsPolicy",
                    builder=> builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed(host=>true)
                    .AllowCredentials()
                    );
            });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "OrdersApi", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OrdersApi v1"));
            }

            app.UseCors("CorsPolicy");

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
