using Streaming.Crystal.Model.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Streaming.Crystal.Model.Type;

namespace Streaming.Crystal.Model.Model.CMP
{
    public class BookImmutable : MessageBase
    {
        public BookImmutable() : base(MarketMessageType.BOOK)
        {
        }

        public override string Id
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
