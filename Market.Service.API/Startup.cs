using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Market.Service.API.Websocket;
using Streaming.Crystal.Connection.Manager;
using Streaming.Crystal.Notification.PostCentral;
using Streaming.Common.Util.Contract;
using Streaming.Crystal.Notification.LocalThread;
using Streaming.Crystal.Notification.Contract;
using Streaming.Crystal.Notification.Faced;

namespace Market.Service.API
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
            services.AddMvc();
            AddDependencyInjectionServicesMkt(services);
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4 * 1024
            };

            app.UseWebSockets(webSocketOptions);

            app.UseMvc();
            app.UseMiddleware<WebsocketMiddleware>();
        }

        private void AddDependencyInjectionServicesMkt(IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            ILogger<PostmanThread> logger = serviceProvider.GetService<ILogger<PostmanThread>>();

            PostmanThread instance = new PostmanThread(logger);
            
            services.AddSingleton<IQueueMessageBox, PostmanThread>(provider => instance);
            services.AddSingleton<IRegisterChannelHandle, PostmanThread>(provider => instance);
            services.AddSingleton<CrystalConnectionMessageHandle, DirectNoficationMessage>();
            services.AddSingleton<IConnectionManager, ConnectionManager>();
            services.AddSingleton<IMarketStreamingFaced, MarketStreamingFaced>();
        }
    }
}
