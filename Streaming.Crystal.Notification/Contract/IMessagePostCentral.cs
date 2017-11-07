using Streaming.Crystal.Model.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Streaming.Crystal.Notification.Contract
{
    public interface IMessagePostCentral
    {
        bool AllocatePostmanThread(string symbol, MarketMessageType type);
        void CreateThreadPool();
        void PostMessage(string message);
        void LoadPoolContext();
    }
}
