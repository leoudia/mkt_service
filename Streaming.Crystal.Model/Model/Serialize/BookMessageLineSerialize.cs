using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Streaming.Crystal.Model.Type;

namespace Streaming.Crystal.Model.Model.Serialize
{
    public class BookMessageLineSerialize
    {
        public string Price { get; protected set; }
        public string Qtd { get; protected set; }
        public string BrokerId { get; protected set; }
        public string DateAndTime { get; protected set; }
        public string OrderId { get; protected set; }
        public int QtdOffers { get; protected set; }

        public BookMessageLineSerialize(string price, string qtd, string brokerId, string dateAndTime, string orderId, int qtdOffers)
        {
            Price = price;
            Qtd = qtd;
            BrokerId = brokerId;
            DateAndTime = dateAndTime;
            OrderId = orderId;
            QtdOffers = qtdOffers;
        }
    }
}
