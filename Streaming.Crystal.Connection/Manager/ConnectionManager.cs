using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Streaming.Crystal.Connection.Connection.Contract;
using Streaming.Crystal.Connection.Model;
using Streaming.Common.Util.List;
using Streaming.Crystal.Connection.Connection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Streaming.Common.Util.Contract;

namespace Streaming.Crystal.Connection.Manager
{
    public class ConnectionManager : IConnectionManager
    {
        private DifusionCredential credential;
        private IList<string> listHost;
        private ConcurrentQueue<ICrystalConnection> connections;
        private CrystalConnectionMessageHandle messageHandle;
        private ILogger<ConnectionManager> logger;
        private ILoggerFactory loggerFactory;
        private IConfiguration config;

        public ConnectionManager(ILoggerFactory loggerFactory, IConfiguration config, CrystalConnectionMessageHandle messageHandle)
        {
            logger = loggerFactory.CreateLogger<ConnectionManager>();
            this.loggerFactory = loggerFactory;
            this.messageHandle = messageHandle;
            this.config = config;

            Load();
        }

        public bool Load()
        {

            string user = config.GetSection("CrystalUserName").Value;
            string pass = config.GetSection("CrystalPassword").Value;

            int port = config.GetSection("CrystalPort").Value != null ? Convert.ToInt16(config.GetSection("CrystalPort").Value) : 81;
            int qtdConnections = config.GetSection("qtd_connections").Value != null ? Convert.ToInt16(config.GetSection("qtd_connections").Value) : 1;

            listHost = config.GetSection("CrystalHosts")
                .GetChildren()
                .Select(x => x.Value)
                .ToList();

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass) || listHost == null || !listHost.Any())
                return false;

            credential = new DifusionCredential(user, pass, "");

            LoadConnections(credential, qtdConnections, port);

            return true;
        }

        private void LoadConnections(DifusionCredential credential, int qtdConnections, int port)
        {
            connections = new ConcurrentQueue<ICrystalConnection>();

            for(var i = 0; i < qtdConnections; i++)
            {
                ICrystalDifusionConnectionBase connBase = new CrystalDifusionConnectionBase(CreateCrystalAddress(port), loggerFactory.CreateLogger<ICrystalDifusionConnectionBase>());

                CrystalDifusionConnectionEvents conn = new CrystalDifusionConnectionEvents(connBase, 
                    messageHandle, 
                    loggerFactory.CreateLogger<CrystalDifusionConnectionEvents>(), 
                    "CrystalConnection " + (i + 1),
                    credential,
                    new CrystalConnectionStatistics());

                connections.Enqueue(conn);
                conn.Connect();
            }

        }

        private CrystalAddress CreateCrystalAddress(int port)
        {
            return new CrystalAddress(NewHosts(), port);
        }

        public ICrystalConnection NextConnection()
        {
            return connections.OrderByDescending(x => x.StatisticsLevel()).Last();
        }

        private ListCircular<string> NewHosts()
        {
            ListCircular<string> listCircularHost = new ListCircular<string>();

            foreach (string host in listHost)
            {
                listCircularHost.Add(host);
            }

            return listCircularHost;
        }
    }
}
