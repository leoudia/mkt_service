using Microsoft.Extensions.Logging;
using Streaming.Crystal.Connection.Connection.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Streaming.Common.Util.Contract;

namespace Streaming.Crystal.Connection.Connection.Events
{
    public class CrystalConnectionReaderEvent
    {
        private CrystalConnectionMessageHandle handle;
        private AtomicRunBoolean run;
        private Thread threadRead;
        private ICrystalDifusionConnectionBase conn;
        private ILogger logger;
        private string connectionId;
        private CrystalConnectionEvent disconnectEvent;

        public CrystalConnectionReaderEvent(CrystalConnectionMessageHandle handle, 
            ICrystalDifusionConnectionBase conn,
            ILogger logger,
            CrystalConnectionEvent disconnectEvent,
            string connectionId)
        {
            this.handle = handle;
            this.conn = conn;
            this.logger = logger;
            this.disconnectEvent = disconnectEvent;
            this.connectionId = connectionId;
        }

        private void ReadConnection()
        {
            try
            {
                while (run.Run)
                {   
                    string msg = conn.Read();
                    if (!string.IsNullOrEmpty(msg))
                    {
                        handle.Arrived(msg);
                    }
                }

                logger.LogInformation("thread read end: " + this.connectionId);
            }
            catch(Exception e)
            {
                logger.LogError("Error read Crystal Connection", e);
                disconnectEvent?.Invoke(false, new EventArgs());
            }
        }

        public void StartReader()
        {
            run = new AtomicRunBoolean();
            if (IsThreadNotAlive())
            {
                threadRead = new Thread(ReadConnection);
                threadRead.Name = "CrystalDifusionReaderThread-" + connectionId;

                threadRead.Start();
            }
        }

        private bool IsThreadNotAlive()
        {
            try
            {
                return threadRead == null || (!threadRead.IsAlive);
            }
            catch (Exception)
            {

            }

            return true;
        }

        public void Stop()
        {
            if(threadRead != null)
            {
                try
                {
                    Interlocked.Exchange(ref run, new AtomicRunBoolean(false));
                }catch(Exception e)
                {
                    logger.LogError("stop", e);
                }
            }
        }

        public bool IsRead
        {
            get { return run.Value; }
        }
    }
}
