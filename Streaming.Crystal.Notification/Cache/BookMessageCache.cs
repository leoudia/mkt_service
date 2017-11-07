using Streaming.Crystal.Model.Model.CMP;
using Streaming.Crystal.Model.Model.Serialize;
using Streaming.Crystal.Notification.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Streaming.Crystal.Notification.Cache
{
    public class BookMessageCache : IMessageCache<MinibookMessage, BookMessageSerialize>
    {
        private string id;
        private int size;
        private string stock;
        private BookDirectionMemory bookBuy;
        private BookDirectionMemory bookSell;

        public BookMessageCache(int size, string stock)
        {
            this.size = size;
            this.stock = stock;
            id = stock + ":" + size;

            bookBuy = new BookDirectionMemory(size, stock, BookDirectionType.BUY);
            bookSell = new BookDirectionMemory(size, stock, BookDirectionType.SELL);
        }
        
        public string Id
        {
            get
            {
                return id;
            }
        }

        public BookMessageSerialize Add(MinibookMessage message)
        {
            if (!string.IsNullOrEmpty(message.BookHeader.Direction))
            {
                var book = message.BookHeader.Direction.Equals("A") ? bookBuy : bookSell;

                switch (message.BookCommandType)
                {
                    case BookCommandType.Add:
                        book.AddLine(message);
                        break;
                    case BookCommandType.Update:
                        book.UpdateLine(message);
                        break;
                }
            }
            else
            {
                if (message.BookCommandType.Equals(BookCommandType.Delete))
                {
                    DeleteAllLines(message);
                }
                else if (message.BookCommandType.Equals(BookCommandType.Error))
                {
                    bookBuy.EndLoadCache = true;
                    bookSell.EndLoadCache = true;
                }
            }

            return Snapshot();
        }

        public BookMessageSerialize Snapshot()
        {
            BookMessageSerialize msgSerialize = new BookMessageSerialize(stock, bookBuy.EndLoadCache)
            {
                Sell = bookSell.SnapShotMiniBook(),
                Buy = bookBuy.SnapShotMiniBook()
            };

            return msgSerialize;
        }

        private void DeleteAllLines(MinibookMessage message)
        {
            BookDelete book = (BookDelete)message.BookHeader;
            if (book.DeleteType == BookDirectionMemory.ALL_LINES)
            {
                Clear();
            }
        }

        public void Clear()
        {
            bookSell.Clear();
            bookBuy.Clear();
        }

        public bool CompareCache(string stock, int size)
        {
            return this.stock.Equals(stock) && this.size == size;
        }

        public bool CompareCache(string stock)
        {
            return false;
        }
    }
}
