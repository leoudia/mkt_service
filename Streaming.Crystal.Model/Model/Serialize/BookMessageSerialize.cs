using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Streaming.Crystal.Model.Type;

namespace Streaming.Crystal.Model.Model.Serialize
{
    public class BookMessageSerialize : MessageSerialize
    {
        public IList<BookMessageLineSerialize> Buy { get; set; }
        public IList<BookMessageLineSerialize> Sell { get; set; }
        private string stock;
        
        public BookMessageSerialize(string stock, bool isEndLoad) : base(MarketMessageType.MINI_BOOK)
        {
            this.stock = stock;
            IsEndLoad = isEndLoad;
        }

        public override string Id
        {
            get
            {
                return stock;
            }
        }

        public bool IsEndLoad { get; protected set; }

        public override string Serialize()
        {
            throw new NotImplementedException();
        }
    }
}
