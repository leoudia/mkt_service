using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Streaming.Crystal.Connection.Connection.Contract
{
    public interface ICrystalConnection
    {
        event CrystalConnectionEvent Connected;
        void Write(string data);
        bool IsConnected();
        long StatisticsLevel();
    }
}
