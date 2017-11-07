using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Streaming.Crystal.Notification.Contract
{
    public interface IQueueMessageBox
    {
        bool Enqueue(string messag);
    }
}
