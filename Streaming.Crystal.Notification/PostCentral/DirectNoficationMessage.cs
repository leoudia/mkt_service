using Microsoft.Extensions.Logging;
using Streaming.Common.Util.Contract;
using Streaming.Crystal.Notification.Contract;
using Streaming.Crystal.Notification.LocalThread;
using System;
using System.Collections.Generic;
using System.Text;

namespace Streaming.Crystal.Notification.PostCentral
{
    public class DirectNoficationMessage : CrystalConnectionMessageHandle
    {
        private IQueueMessageBox postmanThread;

        public DirectNoficationMessage(IQueueMessageBox postmanThread)
        {

            this.postmanThread = postmanThread;
        }

        public void Arrived(string message) => postmanThread.Enqueue(message);
    }
}
