using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Streaming.Crystal.Connection.Connection.Contract
{
    public interface ICrystalConnectionStatistics
    {
        void ChangeMessage();
        long StatisticsLevel();
    }
}
