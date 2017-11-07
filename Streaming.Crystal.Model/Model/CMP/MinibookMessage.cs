using Streaming.Crystal.Model.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Streaming.Crystal.Model.Type;

namespace Streaming.Crystal.Model.Model.CMP
{
    public enum BookCommandType
    {
        Add, Update, Delete, Error
    }

    public class MinibookMessage : MessageBase
    {
        public BookHeader BookHeader;
        private string stock;

        public MinibookMessage(string stock, BookHeader header) : base(MarketMessageType.MINI_BOOK)
        {
            this.stock = stock;
            BookHeader = header;
        }

        public override string Id
        {
            get
            {
                return stock;
            }
        }

        public BookCommandType BookCommandType
        {
            get
            {
                return BookHeader.BookCommandType;
            }
        }
    }
}
