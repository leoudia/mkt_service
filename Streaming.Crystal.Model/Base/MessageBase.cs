using Streaming.Crystal.Model.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Streaming.Crystal.Model.Base
{
    public abstract class MessageBase
    {
        private MarketMessageType type;
        
        public MessageBase(MarketMessageType type)
        {
            this.type = type;
        }

        public abstract string Id { get; }

        public MarketMessageType Type
        {
            get
            {
                return type;
            }
        }
    }
}
