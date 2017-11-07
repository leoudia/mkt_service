using Streaming.Crystal.Model.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Streaming.Crystal.Model.Base
{
    public abstract class MessageBae
    {
        private MarketMessageType type;
        
        public MessageBae(MarketMessageType type)
        {
            this.type = type;
        }

        public MarketMessageType Type
        {
            get
            {
                return type;
            }
        }

        public abstract string Serialize();
    }
}
