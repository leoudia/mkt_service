using Streaming.Common.Util.List;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Streaming.Crystal.Connection.Manager
{
    public class CrystalAddress
    {
        private ListCircular<string> hosts;
        private int port;

        public CrystalAddress(ListCircular<string> hosts, int port)
        {
            if (hosts == null || port == 0)
                throw new ArgumentException();

            this.hosts = hosts;
            this.port = port;
        }

        public int GetPort()
        {
            return port;
        }

        public string GetCurrentHost()
        {
            return hosts.GetCurrent();
        }

        public string GetHost()
        {
            return hosts.Get();
        }
    }
}
