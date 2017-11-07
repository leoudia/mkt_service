using System;
using Streaming.Crystal.Model.Type;
using System.Collections.Generic;

namespace Streaming.Crystal.Model.Model.Serialize
{
    public class QuoteMessageSerialize : MessageSerialize
    {
        private string Stock;

        public bool Error { get; protected set; }

        public Dictionary<int, string> Fields { get; }

        public QuoteMessageSerialize(string stock, bool error, Dictionary<int, string> fields) : base(MarketMessageType.QUOTE)
        {
            Stock = stock;
            Error = error;
            Fields = fields;
        }

        public override string Id
        {
            get
            {
                return Stock;
            }
        }

        public override string Serialize()
        {
            throw new NotImplementedException();
        }
    }
}
