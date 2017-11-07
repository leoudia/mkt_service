using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Streaming.Crystal.Connection.Connection.Contract
{
    public interface ICrystalDifusionConnectionEvents
    {
        event CrystalConnectionEvent Connected;
        event CrystalConnectionEvent Disconnected;
        event CrystalConnectionEvent FailConnect;

        Task<bool> Connect();

        void Stop();

        void Write(string data);
    }

    public delegate void CrystalConnectionEvent(bool connected, EventArgs e);
}
