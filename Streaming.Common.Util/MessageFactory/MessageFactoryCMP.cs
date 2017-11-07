using System;
using System.Collections.Generic;
using System.Text;

namespace Streaming.Common.Util.MessageFactory
{
    public static class MessageFactoryCMP
    {
        public static string SubscribeMiniBook(string stock)
        {
            return $"mbq {stock.ToLower()}";
        }

        public static string UnSubscribeMiniBook(string stock)
        {
            return $"umb {stock.ToLower()}";
        }
    }
}
