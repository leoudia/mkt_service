using Streaming.Crystal.Notification.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Streaming.Crystal.Model.Base;
using System.Net.WebSockets;
using System.Threading;
using System.Text;
using Market.Service.API.Websocket.Command;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace Market.Service.API.Websocket.User
{
    public class UserChannelNotificationWS : IUserChannelNotification
    {
        private const int BUFFER_SIZE = 1024 * 4;
        private WebSocket webSocket;
        private string id;
        private IMarketStreamingFaced marketFaced;
        private ILogger<UserChannelNotificationWS> logger;

        public UserChannelNotificationWS(string id, WebSocket webSocket, IMarketStreamingFaced marketFaced, ILogger<UserChannelNotificationWS> logger)
        {
            this.id = id;
            this.webSocket = webSocket;
            this.marketFaced = marketFaced;
            this.logger = logger;
        }

        public async Task ReadStream()
        {
            var buffer = new byte[BUFFER_SIZE];
            WebSocketReceiveResult result = null;

            do
            {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.EndOfMessage)
                {
                    Process(UTF8Encoding.UTF8.GetString(buffer));
                    buffer = new byte[BUFFER_SIZE];
                }
            } while (!result.CloseStatus.HasValue);
        }

        private void Process(string strCommand)
        {
            try
            {
                BookCommand command = JsonConvert.DeserializeObject<BookCommand>(strCommand);

                switch (command.CommandType)
                {
                    case BookCommand.SubscribeType.SUBSCRIBE:
                        marketFaced.SubscribeMinibook((IUserChannelNotification)this, command.Stock);
                        break;
                    case BookCommand.SubscribeType.UN_SUBSCRIBE:
                        marketFaced.UnSubscribeMinibook((IUserChannelNotification)this, command.Stock);
                        break;
                }
            }
            catch(Exception e)
            {
                logger.LogError("Process", e);
            }
        }

        public void Arrived(MessageBase message)
        {
            try
            {
                string jsonMsg = JsonConvert.SerializeObject(message);

                var buffer = UTF8Encoding.UTF8.GetBytes(jsonMsg);

                webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch(Exception e)
            {
                logger.LogError("Arrived", e);
            }
        }

        public bool Equals(IUserChannelNotification other)
        {
            return other != null && UserId().Equals(other.UserId());
        }

        public bool Equals(IUserChannelNotification x, IUserChannelNotification y)
        {
            return y != null && x != null && x.UserId().Equals(y.UserId());
        }

        public int GetHashCode(IUserChannelNotification obj)
        {
            return UserId().GetHashCode();
        }

        public string UserId()
        {
            return id;
        }
    }
}
