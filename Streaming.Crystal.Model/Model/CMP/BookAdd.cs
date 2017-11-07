using Streaming.Crystal.Model.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Streaming.Crystal.Model.Model.CMP
{
    public class BookAdd : BookHeader
    {
        public const int INDEX_POSITION = 3;
        public const int INDEX_DIRECTION = 4;
        public const int INDEX_PRICE = 5;
        public const int INDEX_QTD = 6;
        public const int INDEX_BROKER_ID = 7;
        public const int INDEX_DATE_AND_TIME = 8;
        public const int INDEX_ORDER_ID = 9;
        public const int QTD_OFFERS = 10;

        public string Price { get; protected set; }
        public string Qtd { get; protected set; }
        public string BrokerId { get; protected set; }
        public string DateAndTime { get; protected set; }
        public string OrderId { get; protected set; }
        public int QtdOffers { get; protected set; }

        public BookAdd(string stock, int position,
            string direction,
            string price,
            string qtd,
            string brokerId,
            string dateAndTime,
            string orderId,
            int qtdOffers) : this(stock, position, direction, price, qtd, brokerId, dateAndTime, orderId, qtdOffers, BookCommandType.Add){}


        public BookAdd(string stock, int position, 
            string direction, 
            string price, 
            string qtd, 
            string brokerId, 
            string dateAndTime,
            string orderId,
            int qtdOffers,
            BookCommandType commandType) : base(commandType)
        {
            this.stock = stock;
            Position = position;
            Direction = direction;
            Price = price;
            Qtd = qtd;
            BrokerId = brokerId;
            DateAndTime = dateAndTime;
            OrderId = orderId;
            QtdOffers = qtdOffers;
        }
    }
}
