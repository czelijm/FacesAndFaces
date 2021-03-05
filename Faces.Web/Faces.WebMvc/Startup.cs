using Faces.WebMvc.RestClients;
using Faces.WebMvc.Services;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Faces.WebMvc
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

            services.Configure<Settings.AppSettings>(Configuration);

            services.AddMassTransit();

            services.AddSingleton(provider=>Bus.Factory.CreateUsingRabbitMq
                (cfg => 
                    {
                        cfg.Host(
                             Messaging.InterfacesConstants.Constants.RabbitMqMassTransitConstants.HostName, 
                            "/", 
                            h=> { }
                        );//blank for default parameters
                        services.AddSingleton<IBusControl>(provider => provider.GetRequiredService<IBusControl>());
                        services.AddSingleton<IHostedService, BusService>();
                    }
                    
                )//multiple transports can be supported via specific interface
            );

            services.AddHttpClient<IOrderManagementApi,OrderManagementApi>();


            services.AddControllersWithViews();

            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
