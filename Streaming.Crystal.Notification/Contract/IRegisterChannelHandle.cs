using Streaming.Crystal.Model.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Streaming.Crystal.Notification.Contract
{
    public interface IRegisterChannelHandle
    {
        bool AddChannelHandle(string id, MarketMessageType type, IUserChannelNotification channel);
        bool RemoveChannelHandle(string id, MarketMessageType type, IUserChannelNotification channel);
    }
}
