using Streaming.Crystal.Model.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Streaming.Crystal.Model.Type;

namespace Streaming.Crystal.Model.Model.CMP
{
    public class QuoteMessage : MessageBase
    {
        private string stock;

        public bool Error { get; protected set; }

        public Dictionary<int, string> Fields { get;}

        public QuoteMessage(string stock, Dictionary<int, string> fields, bool error = false) : base(MarketMessageType.QUOTE)
        {
            this.stock = stock;
            Fields = fields;
            Error = error;
        }

        public override string Id
        {
            get
            {
                return stock;
            }
        }
    }
}
