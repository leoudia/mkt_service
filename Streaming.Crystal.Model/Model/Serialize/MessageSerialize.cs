using Streaming.Crystal.Model.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Streaming.Crystal.Model.Type;

namespace Streaming.Crystal.Model.Model.Serialize
{
    public abstract class MessageSerialize : MessageBase
    {
        public MessageSerialize(MarketMessageType type) : base(type)
        {
        }

        public abstract string Serialize();
    }
}
