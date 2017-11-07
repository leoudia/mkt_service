using Streaming.Crystal.Model.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Streaming.Crystal.Notification.Contract
{
    public interface IUserChannelNotification : IEquatable<IUserChannelNotification>, IEqualityComparer<IUserChannelNotification>
    {
        void Arrived(MessageBase message);

        string UserId();
    }
}
