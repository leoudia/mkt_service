using Streaming.Crystal.Model.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Streaming.Crystal.Notification.Parse
{
    public interface IParseMessageProtocolCMP<TMessageBase> where TMessageBase : MessageBase
    {
        TMessageBase Parse(string message);

        TMessageBase ParseError(string message);
    }
}

