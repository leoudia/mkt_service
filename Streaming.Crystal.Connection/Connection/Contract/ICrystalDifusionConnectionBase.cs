using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Streaming.Crystal.Connection.Connection.Contract
{
    public interface ICrystalDifusionConnectionBase
    {
        Task<bool> Open();
        void Stop();
        void Write(string data);
        string Read();
        bool IsConnected();
    }
}
