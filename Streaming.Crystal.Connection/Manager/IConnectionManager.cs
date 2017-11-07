using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Streaming.Common.Util.Contract;
using Streaming.Crystal.Connection.Connection.Contract;

namespace Streaming.Crystal.Connection.Manager
{
    public interface IConnectionManager
    {
        bool Load();
        ICrystalConnection NextConnection();
    }
}
