using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Streaming.Crystal.Model.Type;

namespace Streaming.Crystal.Model.Model.CMP
{
    public class BookUpdate : BookAdd
    {
        public const int INDEX_OLD_POSITION_UP = 4;
        public const int INDEX_DIRECTION_UP = 5;
        public const int INDEX_PRICE_UP = 6;
        public const int INDEX_QTD_UP = 7;
        public const int INDEX_BROKER_ID_UP = 8;
        public const int INDEX_DATE_AND_TIME_UP = 9;
        public const int INDEX_ORDER_ID_UP = 10;
        public const int QTD_OFFERS_UP = 11;

        public int OldPosition { get; protected set; }

        public BookUpdate(string stock, int position, int oldPosition, string direction, string price, string qtd, string brokerId, string dateAndTime, string orderId, int qtdOffers) 
            : base(stock, position, direction, price, qtd, brokerId, dateAndTime, orderId, qtdOffers, BookCommandType.Update)
        {
            OldPosition = oldPosition;
        }
    }
}
