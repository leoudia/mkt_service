using Microsoft.Extensions.Logging;
using Streaming.Crystal.Connection.Connection.Contract;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Streaming.Crystal.Connection.Connection.Events
{
    public class CrystalConnectionWriterEvent
    {
        private AtomicRunBoolean run;
        private Thread threadWrite;
        private ICrystalDifusionConnectionBase conn;
        private ILogger logger;
        private string connectionId;
        private CrystalConnectionEvent disconnectEvent;
        private BlockingCollection<string> queue;

        public CrystalConnectionWriterEvent(ICrystalDifusionConnectionBase conn, 
            ILogger logger, 
            CrystalConnectionEvent disconnectEvent,
            BlockingCollection<string> queue,
            string connectionId)
        {
            this.conn = conn;
            this.logger = logger;
            this.disconnectEvent = disconnectEvent;
            this.queue = queue;
            this.connectionId = connectionId;

            run = new AtomicRunBoolean();
        }

        private void Write()
        {
            string msg = null;

            try
            {
                while (run.Run)
                {
                    msg = queue.Take();
                    conn.Write(msg);
                }

                logger.LogInformation("thread write end");
            }catch(Exception e)
            {
                if(!string.IsNullOrEmpty(msg))
                    queue.Add(msg);

                logger.LogError("crystal writer error", e);
                disconnectEvent?.Invoke(false, new EventArgs());
            }
        }

        public void Stop()
        {
            if(threadWrite != null)
            {
                try
                {
                    Interlocked.Exchange(ref run, new AtomicRunBoolean(false));
                }
                catch(Exception e)
                {
                    logger.LogError("stop", e);
                }
            }
        }
        
        public void StartWriter()
        {
            run = new AtomicRunBoolean();
            if (IsThreadNotAlive())
            {
                threadWrite = new Thread(Write);
                threadWrite.Name = "CrsytalThreadWriter-" + connectionId;

                threadWrite.Start();
            }
        }

        private bool IsThreadNotAlive()
        {
            try
            {
                return threadWrite == null || !threadWrite.IsAlive;
            }catch(Exception e)
            {
                logger.LogError("IsThreadNotAlive", e);
            }

            return true;
        }
    }
}
