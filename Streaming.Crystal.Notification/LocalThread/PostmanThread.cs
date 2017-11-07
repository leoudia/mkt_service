using Streaming.Crystal.Model.Type;
using Streaming.Crystal.Notification.Contract;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Streaming.Crystal.Notification.Model;
using Streaming.Crystal.Notification.Parse.Book;
using Streaming.Crystal.Model.Base;
using Streaming.Crystal.Notification.Cache;
using Streaming.Crystal.Model.Model.CMP;
using Streaming.Crystal.Model.Model.Serialize;

namespace Streaming.Crystal.Notification.LocalThread
{
    public class PostmanThread : IQueueMessageBox, IRegisterChannelHandle
    {
        private Thread threadNotification;
        private BlockingCollection<string> queue;
        private ConcurrentDictionary<CommandKey, IEnumerable<IUserChannelNotification>> messageListeners;
        private ILogger<PostmanThread> logger;
        private BookParseProtocolCMP bookParse;
        private IDictionary<string, BookMessageCache> bookCache;
        private static readonly int BOOK_SIZE = 5;

        public PostmanThread(ILogger<PostmanThread> logger)
        {
            this.logger = logger;
            queue = new BlockingCollection<string>();
            messageListeners = new ConcurrentDictionary<CommandKey, IEnumerable<IUserChannelNotification>>();

            bookCache = new ConcurrentDictionary<string, BookMessageCache>();
            bookParse = new BookParseProtocolCMP();

            threadNotification = new Thread(ReadQueue);
            threadNotification.Start();
        }

        private void ReadQueue()
        {
            string msg = "";

            while (true)
            {
                try
                {
                    msg = queue.Take();
                    var book = ParseBook(msg);
                    var bookSerialize = AddBookCache(book);
                    NotifyBook(bookSerialize);
                }catch(Exception e)
                {
                    logger.LogError($"notify: {msg}");
                    logger.LogError("ReadQueue", e);
                }
            }
        }

        private BookMessageSerialize AddBookCache(MessageBase book)
        {
            if (!bookCache.TryGetValue(book.Id, out BookMessageCache cache))
            {
                cache = new BookMessageCache(BOOK_SIZE, book.Id);
                while (bookCache.TryAdd(book.Id, cache)) ;
            }

            return cache.Add(book as MinibookMessage);
        }

        private void NotifyBook(BookMessageSerialize book)
        {
            if ( IsNotifyMessage(book) && 
                messageListeners.TryGetValue(new CommandKey(book.Type, book.Id), out IEnumerable<IUserChannelNotification> listeners))
            {

                foreach (var listener in listeners)
                {
                    listener.Arrived(book);
                }
            }
        }

        private bool IsNotifyMessage(BookMessageSerialize book)
        {
            return book.IsEndLoad;
        }

        private MessageBase ParseBook(string msg)
        {   
            return bookParse.Parse(msg);
        }

        public bool Enqueue(string message)
        {
            if (string.IsNullOrEmpty(message))
                return false;

            queue.Add(message);

            return true;
        }

        public bool AddChannelHandle(string id, MarketMessageType type, IUserChannelNotification channel)
        {
            CommandKey key = new CommandKey(type, id);

            bool firstHandle = false;

            if (!messageListeners.TryGetValue(key, out IEnumerable<IUserChannelNotification> list))
            {
                list = new ConcurrentBag<IUserChannelNotification>();
                messageListeners.TryAdd(key, list);

                firstHandle = true;
            }
            else
            {
                if (bookCache.TryGetValue(id, out BookMessageCache cache))
                    channel.Arrived(cache.Snapshot());
            }

            if (!list.Contains(channel))
            {
                ((ConcurrentBag<IUserChannelNotification>)list).Add(channel);
            }
            
            return firstHandle;
        }

        public bool RemoveChannelHandle(string id, MarketMessageType type, IUserChannelNotification channel)
        {
            CommandKey key = new CommandKey(type, id);

            if (messageListeners.TryGetValue(key, out IEnumerable<IUserChannelNotification> list))
            {
                var tmp = list.ToList();
                tmp.Remove(channel);
                
                if (!tmp.Any())
                {
                    messageListeners.Remove(key, out list);

                    return true;
                }
            }

            return false;
        }

        public void AddChannelHandle(string v, MarketMessageType mINI_BOOK, Func<object> p)
        {
            throw new NotImplementedException();
        }
    }
}
