using Streaming.Crystal.Model.Model.CMP;
using Streaming.Crystal.Model.Model.Serialize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Streaming.Crystal.Notification.Model
{
    public class BookDirectionMemory
    {
        private IList<BookAdd> cacheDirection;
        private const int CURRENT_OFFER_INDEX = 1;
        private const int ALL_TOP_AND_THIS_OFFERS_INDEX = 2;
        public static int ALL_LINES = 3;
        private int Size { get; }

        private string stock;

        private BookDirectionType directionType;

        public BookDirectionMemory(int size, string stock, BookDirectionType directionType)
        {
            cacheDirection = new List<BookAdd>(size);
            Size = size;
            this.stock = stock;
            this.directionType = directionType;
        }

        public bool EndLoadCache { get; set; }

        public void DeleteLineByIndex(int index)
        {
            if (cacheDirection.Count > index)
                cacheDirection.RemoveAt(index);
        }

        public void DeleteLine(MinibookMessage message)
        {
            BookDelete book = (BookDelete)message.BookHeader;
            switch(book.DeleteType)
            {
                case CURRENT_OFFER_INDEX:
                    DeleteLineByIndex(book.Position);
                    break;
                case ALL_TOP_AND_THIS_OFFERS_INDEX:
                    DeleteRangeLines(0, (book.Position + 1));
                    break;
            }
        }

        private void DeleteRangeLines(int startPos, int count)
        {
            if(cacheDirection.Count >= count)
                ((List<BookAdd>)cacheDirection).RemoveRange(startPos, count);
        }

        public void AddLine(MinibookMessage message)
        {
            if (IsIndexValid(message.BookHeader.Position))
            {
                BookAdd book = (BookAdd)message.BookHeader;
                cacheDirection.Insert(book.Position, book);

                NormalizeBook();
            }
        }

        private void NormalizeBook()
        {
            if(cacheDirection.Count > Size)
            {
                ((List<BookAdd>)cacheDirection).RemoveRange(Size - 1, (Size - cacheDirection.Count) );
            }
        }

        private bool IsIndexValid(int position)
        {
            return cacheDirection.Count >= position;
        }

        public void UpdateLine(MinibookMessage message)
        {
            BookUpdate book = (BookUpdate)message.BookHeader;

            DeleteLineByIndex(book.OldPosition);
        }

        public IList<BookMessageLineSerialize> SnapShotMiniBook()
        {
            var list = cacheDirection.Select(x => new BookMessageLineSerialize(x.Price, x.Qtd, x.BrokerId, x.DateAndTime, x.OrderId, x.QtdOffers)).ToList();
            return list;
        }

        public void Clear()
        {
            EndLoadCache = false;
            cacheDirection.Clear();
        }
    }
}
