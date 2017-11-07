using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Streaming.Crystal.Model.Model.CMP
{
    public class BookError : BookHeader
    {
        public const int ERROR_CODE_INDEX = 3;
        public int CodeError { get; protected set; }
        public BookError(string stock, int codeError) : base(BookCommandType.Error)
        {
            this.stock = stock;
            CodeError = codeError;
        }
    }
}
