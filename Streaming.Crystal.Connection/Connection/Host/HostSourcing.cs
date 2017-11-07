using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Streaming.Crystal.Connection.Connection.Host
{
    public class HostSourcing
    {
        private IList<string> hosts;


        public HostSourcing(IList<string> hosts)
        {
            this.hosts = hosts;
        }


        public string GetHost()
        {

            return hosts.First();
        }
    }
}
