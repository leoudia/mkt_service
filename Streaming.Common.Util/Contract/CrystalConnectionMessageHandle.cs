using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Streaming.Common.Util.Contract
{
    public interface CrystalConnectionMessageHandle
    {
        void Arrived(string message);
    }
}
