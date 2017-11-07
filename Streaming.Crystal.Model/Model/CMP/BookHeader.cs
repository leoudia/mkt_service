using Streaming.Crystal.Model.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Streaming.Crystal.Model.Type;

namespace Streaming.Crystal.Model.Model.CMP
{
    public abstract class BookHeader
    {
        protected string stock;
        public BookCommandType BookCommandType;

        public BookHeader(BookCommandType bookCommandType)
        {
            BookCommandType = bookCommandType;
        }

        public int Position { get; protected set; }
        public string Direction { get; protected set; }
    }
}
