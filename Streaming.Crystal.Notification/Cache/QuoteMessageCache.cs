using System;
using System.Collections.Generic;
using Streaming.Crystal.Model.Model.CMP;
using Streaming.Crystal.Model.Model.Serialize;

namespace Streaming.Crystal.Notification.Cache
{
    public class QuoteMessageCache : IMessageCache<QuoteMessage, QuoteMessageSerialize>
    {
        private string Stock { get; }

        private Dictionary<int, string> cache;

        public QuoteMessageCache(string stock)
        {
            Stock = stock;

            cache = new Dictionary<int, string>();
        }

        public string Id
        {
            get
            {
                return Stock;
            }
        }

        public QuoteMessageSerialize Add(QuoteMessage message)
        {
            foreach(KeyValuePair<int, string> pair in message.Fields)
            {
                if (!string.IsNullOrEmpty(pair.Value))
                {
                    cache.Add(pair.Key, pair.Value);
                }
            }

            return new QuoteMessageSerialize(message.Id, message.Error, message.Fields);
        }

        public void Clear()
        {
            cache.Clear();
        }

        public bool CompareCache(string stock, int size)
        {
            return CompareCache(stock);
        }

        public bool CompareCache(string stock)
        {
            return Stock.Equals(stock);
        }

        public BookMessageSerialize Snapshot()
        {
            throw new NotImplementedException();
        }
    }
}
