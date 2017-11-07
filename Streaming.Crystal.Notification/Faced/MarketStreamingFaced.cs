using Streaming.Common.Util.MessageFactory;
using Streaming.Crystal.Connection.Manager;
using Streaming.Crystal.Notification.Contract;
using Streaming.Crystal.Notification.LocalThread;
using System;
using System.Collections.Generic;
using System.Text;

namespace Streaming.Crystal.Notification.Faced
{
    public class MarketStreamingFaced : IMarketStreamingFaced
    {
        private IConnectionManager manager;
        private IRegisterChannelHandle registerChannel;
        public MarketStreamingFaced(IConnectionManager manager, IRegisterChannelHandle registerChannel)
        {
            this.manager = manager;
            this.registerChannel = registerChannel;
        }

        public void SubscribeMinibook(IUserChannelNotification userChannel, string stock)
        {
            if (IsValidParameters(userChannel, stock))
            {
                if (registerChannel.AddChannelHandle(stock.ToUpper(), Crystal.Model.Type.MarketMessageType.MINI_BOOK, userChannel))
                    manager.NextConnection().Write(MessageFactoryCMP.SubscribeMiniBook(stock));
            }
        }

        private static bool IsValidParameters(IUserChannelNotification userChannel, string stock)
        {
            return !string.IsNullOrEmpty(stock) && userChannel != null && !string.IsNullOrEmpty(userChannel.UserId());
        }

        public void UnSubscribeMinibook(IUserChannelNotification userChannel, string stock)
        {
            if (IsValidParameters(userChannel, stock))
            {
                if (registerChannel.RemoveChannelHandle(stock.ToUpper(), Crystal.Model.Type.MarketMessageType.MINI_BOOK, userChannel))
                    manager.NextConnection().Write(MessageFactoryCMP.UnSubscribeMiniBook(stock));
            }
        }
    }
}
