using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Streaming.Crystal.Model.Type;

namespace Streaming.Crystal.Model.Model.CMP
{
    public class BookDelete : BookHeader
    {
        public const int INDEX_TYPE = 3;
        public const int INDEX_DIRECTION = 4;
        public const int INDEX_POSITION = 5;
        public static int INDEX_QTD_OFFERS = 6;
        public static int INDEX_QTD_OFFERS_4 = 4;

        public int DeleteType { get; protected set; }

        public int QtdOffers { get; protected set; }

        public BookDelete(string stock, int deleteType, string direction, int position, int qtdOffers) : base(BookCommandType.Delete)
        {
            this.stock = stock;
            DeleteType = deleteType;
            Direction = direction;
            Position = position;
            QtdOffers = qtdOffers;
        }

        public BookDelete(string stock, int deleteType, int qtdOffers) : base(BookCommandType.Delete)
        {
            this.stock = stock;
            DeleteType = deleteType;
            QtdOffers = qtdOffers;
        }
    }
}
