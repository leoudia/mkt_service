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
    public class CrystalConnectionWatcher
    {
        private BlockingCollection<string> queue;
        private DateTime lastMessageTime;
        private Thread threadWatcher;
        private CrystalConnectionEvent disconnectEvent;
        private TimeSpan toleranceTime;
        private AtomicRunBoolean run;
        private ILogger logger;

        public CrystalConnectionWatcher(BlockingCollection<string> queue, CrystalConnectionEvent disconnectEvent, ILogger logger)
        {
            this.queue = queue;
            this.disconnectEvent = disconnectEvent;
            this.logger = logger;
            
            lastMessageTime = DateTime.Now;

            toleranceTime = TimeSpan.FromSeconds(6);
        }

        private void Watcher()
        {
            while (run.Run)
            {
                Task.Delay(TimeSpan.FromSeconds(3)).Wait();

                SendAck();
                VerifyLastMessage();
            }
            
            logger.LogInformation("thread watcher end: ");
        }

        private void VerifyLastMessage()
        {
            TimeSpan current = DateTime.Now - lastMessageTime;

            if (current > toleranceTime)
                disconnectEvent?.Invoke(true, new EventArgs());
        }

        public void Start()
        {
            run = new AtomicRunBoolean();
            if (IsThreadNotAlive())
            {
                threadWatcher = new Thread(() =>
                {
                    Watcher();
                });
                threadWatcher.Start();
            }
        }

        private bool IsThreadNotAlive()
        {
            try
            {
                return threadWatcher == null || !threadWatcher.IsAlive;
            }
            catch (Exception)
            {

            }

            return true;
        }

        public void Stop()
        {
            if (threadWatcher != null)
            {
                try
                {
                    Interlocked.Exchange(ref run, new AtomicRunBoolean(false));
                }
                catch (Exception e)
                {
                    logger.LogError("stop", e);
                }
            }
        }

        private void SendAck()
        {
            queue.Add("SYN");
        }

        public bool IsMessageControl(string msg)
        {   
            lastMessageTime = DateTime.Now;

            return msg != null && msg.Equals("ACK") || msg.Equals("SYN");
        }


    }
}
