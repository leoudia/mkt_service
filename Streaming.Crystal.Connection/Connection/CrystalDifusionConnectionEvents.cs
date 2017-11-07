using Microsoft.Extensions.Logging;
using Streaming.Crystal.Connection.Connection.Contract;
using Streaming.Crystal.Connection.Connection.Events;
using Streaming.Crystal.Connection.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Streaming.Common.Util.Contract;

namespace Streaming.Crystal.Connection.Connection
{
    public class CrystalDifusionConnectionEvents : ICrystalDifusionConnectionEvents, CrystalConnectionMessageHandle, ICrystalConnection
    {
        public event CrystalConnectionEvent Connected;
        public event CrystalConnectionEvent Disconnected;
        public event CrystalConnectionEvent FailConnect;

        private ICrystalDifusionConnectionBase conn;
        private ILogger<CrystalDifusionConnectionEvents> logger;
        private readonly BlockingCollection<string> queue = new BlockingCollection<string>();
        private CrystalConnectionReaderEvent crystalReader;
        private CrystalConnectionWriterEvent crystalWriter;
        private CrystalConnectionWatcher crystalWatcher;
        private string connectionId;
        private CrystalConnectionMessageHandle handle;
        private DifusionCredential credential;

        private AtomicRunBoolean reconnect;
        private AtomicRunBoolean watch;
        private AtomicRunBoolean connected;

        private Action<string> currentActionMessageHandle;

        private ICrystalConnectionStatistics connectionStatistics;

        public CrystalDifusionConnectionEvents(ICrystalDifusionConnectionBase conn, 
            CrystalConnectionMessageHandle handle, 
            ILogger<CrystalDifusionConnectionEvents> logger,
            string connectionId,
            DifusionCredential credential,
            ICrystalConnectionStatistics connectionStatistics)
        {
            this.conn = conn;
            this.logger = logger;
            this.connectionId = connectionId;
            this.handle = handle;
            this.credential = credential;

            Disconnected = DisconnectHandle;
            Connected = ConnectedtHandle;

            crystalReader = new CrystalConnectionReaderEvent(this, conn, logger, Disconnected, connectionId);
            crystalWriter = new CrystalConnectionWriterEvent(conn, logger, Disconnected, queue, connectionId);
            crystalWatcher = new CrystalConnectionWatcher(queue, Disconnected, logger);

            reconnect = new AtomicRunBoolean(false);
            watch = new AtomicRunBoolean(false);
            connected = new AtomicRunBoolean(false);

            this.connectionStatistics = connectionStatistics;
        }

        public async Task<bool> Open()
        {
            try
            {
                if (conn.IsConnected())
                {
                    return true;
                }
                else
                {
                    LoginState();
                }

                return await conn.Open();
            }catch(Exception e)
            {
                logger.LogError("Open Crystal Connection Error", e);
            }

            return false;
        }

        private void LoginState()
        {
            currentActionMessageHandle = ProcessLoginMessages;
        }

        private void NotifyMessagesState()
        {
            currentActionMessageHandle = ListenerMessage;
        }

        private void Reconnect()
        {
            if (!reconnect.Value)
            {
                Stop();
                reconnect.Value = true;

                var threadReconnect = new Thread(() =>
                {
                    while (!Open().Result)
                    {
                        Task.Delay(TimeSpan.FromSeconds(2)).Wait();
                    }
                    Start();
                    StartWatchLogin();
                    reconnect.Value = false;
                });

                threadReconnect.Name = "CrystalDifusion-Reconnect";
                threadReconnect.Start();
            }
        }

        public void Stop()
        {
            try
            {
                crystalReader.Stop();
                crystalWriter.Stop();
                crystalWatcher.Stop();
                conn.Stop();
            }catch(Exception e)
            {
                logger.LogError("stop all events connection", e);
            }
        }

        public void Write(string data)
        {
            if(data != null)
                queue.Add(data);
        }

        public void Arrived(string message)
        {
            if(!crystalWatcher.IsMessageControl(message))
                currentActionMessageHandle(message);
        }

        private void ListenerMessage(string message)
        {
            handle.Arrived(message);

            connectionStatistics.ChangeMessage();
        }

        private void ProcessLoginMessages(string message)
        {
            message = message.Trim();
            if (message.ToLower().Equals(CrystalConnectionConstant.START_LOGIN))
            {
                Write(credential.AppName);
            }
            else if (message.ToLower().Equals(CrystalConnectionConstant.USERNAME_KEY))
            {
                Write(credential.User);
            }
            else if (message.ToLower().Equals(CrystalConnectionConstant.PASSWORD_KEY))
            {
                Write(credential.Pass);
            }
            else if (message.ToLower().Equals(CrystalConnectionConstant.LOGGED_CRYSTAL))
            {
                NotifyMessagesState();
                Connected?.Invoke(true, new EventArgs());
            }
            else if (message.ToLower().Equals(CrystalConnectionConstant.FAILED_LOGIN))
            {
                FailConnect?.Invoke(false, new EventArgs());
            }
        }

        private void StartWatchLogin()
        {
            if (!watch.Value)
            {
                watch.Value = true;
                var threadWatchLogin = new Thread(() =>
                {
                    Task.Delay(TimeSpan.FromSeconds(10)).Wait();

                    if (!connected.Value)
                        Reconnect();

                    watch.Value = false;
                });
            }
        }

        private void Start()
        {
            crystalReader.StartReader();
            crystalWriter.StartWriter();
        }

        public void ConnectedtHandle(bool connected, EventArgs e)
        {
            this.connected.Value = connected;
            crystalWatcher.Start();
        }

        public void DisconnectHandle(bool connected, EventArgs e)
        {
            this.connected.Value = connected;
            Reconnect();
        }

        public Task<bool> Connect()
        {
            Reconnect();
            return Task.FromResult<bool>(true);
        }

        public bool IsConnected()
        {
            return this.connected.Value;
        }

        public long StatisticsLevel()
        {
            return connectionStatistics.StatisticsLevel();
        }

        public string Id
        {
            get { return connectionId; }
        }
    }
}
