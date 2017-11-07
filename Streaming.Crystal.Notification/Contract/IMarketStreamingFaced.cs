using System;
using System.Collections.Generic;
using System.Text;

namespace Streaming.Crystal.Notification.Contract
{
    public interface IMarketStreamingFaced
    {
        void SubscribeMinibook(IUserChannelNotification userChannel, string stock);

        void UnSubscribeMinibook(IUserChannelNotification userChannel, string stock);
    }
}
