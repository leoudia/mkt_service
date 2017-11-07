using Streaming.Crystal.Model.Type;
using Streaming.Crystal.Notification.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Streaming.Crystal.Notification.PostCentral
{
    public class MessagePostCentral : IMessagePostCentral
    {

        public bool AllocatePostmanThread(string symbol, MarketMessageType type)
        {
            throw new NotImplementedException();
        }

        public void CreateThreadPool()
        {
            throw new NotImplementedException();
        }

        public void LoadPoolContext()
        {
            throw new NotImplementedException();
        }

        public void PostMessage(string message)
        {
            throw new NotImplementedException();
        }
    }
}
