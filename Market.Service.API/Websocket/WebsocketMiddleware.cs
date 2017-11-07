using Market.Service.API.Websocket.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Streaming.Crystal.Notification.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Market.Service.API.Websocket
{
    public class WebsocketMiddleware
    {
        private RequestDelegate next;
        private IMarketStreamingFaced marketFaced;
        private ILogger<WebsocketMiddleware> logger;
        private ILoggerFactory loggerFactory;

        public WebsocketMiddleware(RequestDelegate next, IMarketStreamingFaced marketFaced, ILoggerFactory loggerFactory)
        {
            this.next = next;
            this.marketFaced = marketFaced;
            this.loggerFactory = loggerFactory;
            logger = loggerFactory.CreateLogger<WebsocketMiddleware>();
        }
        public async Task Invoke(HttpContext context)
        {   
            if (context.Request.Path == "/ws")
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();

                    UserChannelNotificationWS userChannel = new UserChannelNotificationWS(GenerateId(context), 
                        webSocket, marketFaced, loggerFactory.CreateLogger<UserChannelNotificationWS>());
                    await userChannel.ReadStream();
                }
                else
                {
                    context.Response.StatusCode = 400;
                }
            }
            else
            {
                await next(context);
            }
        }

        private static string GenerateId(HttpContext context)
        {
            var sessionFeature = context.Features.Get<ISessionFeature>();

            if (sessionFeature != null && !string.IsNullOrEmpty(context.Session.Id))
            {
                return context.Session.Id;
            }
            else
            {
                Guid guid = Guid.NewGuid();
                
                return guid.ToString();
            }
        }
    }
}
