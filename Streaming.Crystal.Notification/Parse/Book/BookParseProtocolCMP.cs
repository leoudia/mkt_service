using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Streaming.Crystal.Model.Base;
using Streaming.Crystal.Model.Model.CMP;

namespace Streaming.Crystal.Notification.Parse.Book
{
    public class BookParseProtocolCMP : IParseMessageProtocolCMP<MinibookMessage>
    {
        private static readonly int INDEX_STOCK = 1;
        private static readonly int INDEX_COMMAND = 2;

        private const string ADD_COMMAND = "A";
        private const string CANCEL_COMMAND = "D";
        private const string ERROR_COMMAND = "E";
        private const string UPDATE_COMMAND = "U";

        public MinibookMessage Parse(string message)
        {
            BookHeader msg = null;

            string[] columns = message.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

            string stock = columns[INDEX_STOCK];

            switch (columns[INDEX_COMMAND])
            {
                case ADD_COMMAND:
                    return new MinibookMessage(stock, CreateBookAdd(columns, stock));
                case UPDATE_COMMAND:
                    return new MinibookMessage(stock, CreateBookUpdate(columns, stock));
                case CANCEL_COMMAND:
                    return new MinibookMessage(stock, CreateBookDelete(columns, stock));
                case ERROR_COMMAND:
                    return new MinibookMessage(stock, CreateBookError(columns, stock));
            }


            return new MinibookMessage(stock, msg);
        }

        private BookHeader CreateBookError(string[] columns, string stock)
        {
            int codeError = int.Parse(columns[BookError.ERROR_CODE_INDEX]);
            return new BookError(stock, codeError);
        }

        private BookHeader CreateBookDelete(string[] columns, string stock)
        {
            switch (columns.Length)
            {
                case 7:
                    return new BookDelete(stock, int.Parse(columns[BookDelete.INDEX_TYPE]),
                        columns[BookDelete.INDEX_DIRECTION],
                        int.Parse(columns[BookDelete.INDEX_POSITION]),
                        int.Parse(columns[BookDelete.INDEX_QTD_OFFERS]));
                case 6:
                    return new BookDelete(stock, int.Parse(columns[BookDelete.INDEX_TYPE]),
                        columns[BookDelete.INDEX_DIRECTION],
                        int.Parse(columns[BookDelete.INDEX_POSITION]),
                        0);
                case 5:
                    return new BookDelete(stock, int.Parse(columns[BookDelete.INDEX_TYPE]), 
                        int.Parse(columns[BookDelete.INDEX_QTD_OFFERS_4]));
                case 4:
                    return new BookDelete(stock, int.Parse(columns[BookDelete.INDEX_TYPE]), 0);
                default:
                    return null;

            }
        }

        private BookHeader CreateBookUpdate(string[] columns, string stock)
        {
            switch (columns.Length)
            {
                case 10:
                    return new BookUpdate(stock,
                        int.Parse(columns[BookUpdate.INDEX_POSITION]),
                        int.Parse(columns[BookUpdate.INDEX_OLD_POSITION_UP]),
                        columns[BookUpdate.INDEX_DIRECTION_UP],
                        columns[BookUpdate.INDEX_PRICE_UP],
                        columns[BookUpdate.INDEX_QTD_UP],
                        columns[BookUpdate.INDEX_BROKER_ID_UP],
                        columns[BookUpdate.INDEX_DATE_AND_TIME_UP],
                        null,
                        0);
                case 11:
                    return new BookUpdate(stock,
                        int.Parse(columns[BookUpdate.INDEX_POSITION]),
                        int.Parse(columns[BookUpdate.INDEX_OLD_POSITION_UP]),
                        columns[BookUpdate.INDEX_DIRECTION_UP],
                        columns[BookUpdate.INDEX_PRICE_UP],
                        columns[BookUpdate.INDEX_QTD_UP],
                        columns[BookUpdate.INDEX_BROKER_ID_UP],
                        columns[BookUpdate.INDEX_DATE_AND_TIME_UP],
                        columns[BookUpdate.INDEX_ORDER_ID_UP],
                        0);
                case 12:
                    return new BookUpdate(stock,
                        int.Parse(columns[BookUpdate.INDEX_POSITION]),
                        int.Parse(columns[BookUpdate.INDEX_OLD_POSITION_UP]),
                        columns[BookUpdate.INDEX_DIRECTION_UP],
                        columns[BookUpdate.INDEX_PRICE_UP],
                        columns[BookUpdate.INDEX_QTD_UP],
                        columns[BookUpdate.INDEX_BROKER_ID_UP],
                        columns[BookUpdate.INDEX_DATE_AND_TIME_UP],
                        columns[BookUpdate.INDEX_ORDER_ID_UP],
                        int.Parse(columns[BookUpdate.QTD_OFFERS_UP]));
                default:
                    return null;
            }
        }

        private BookHeader CreateBookAdd(string[] columns, string stock)
        {
            switch (columns.Length)
            {
                case 9:
                    return new BookAdd(stock, int.Parse(columns[BookAdd.INDEX_POSITION]), 
                        columns[BookAdd.INDEX_DIRECTION],
                        columns[BookAdd.INDEX_PRICE],
                        columns[BookAdd.INDEX_QTD],
                        columns[BookAdd.INDEX_BROKER_ID],
                        columns[BookAdd.INDEX_DATE_AND_TIME],
                        null,
                        0);
                case 10:
                    return new BookAdd(stock, int.Parse(columns[BookAdd.INDEX_POSITION]),
                        columns[BookAdd.INDEX_DIRECTION],
                        columns[BookAdd.INDEX_PRICE],
                        columns[BookAdd.INDEX_QTD],
                        columns[BookAdd.INDEX_BROKER_ID],
                        columns[BookAdd.INDEX_DATE_AND_TIME],
                        columns[BookAdd.INDEX_ORDER_ID],
                        0);
                case 11:
                    return new BookAdd(stock, int.Parse(columns[BookAdd.INDEX_POSITION]),
                        columns[BookAdd.INDEX_DIRECTION],
                        columns[BookAdd.INDEX_PRICE],
                        columns[BookAdd.INDEX_QTD],
                        columns[BookAdd.INDEX_BROKER_ID],
                        columns[BookAdd.INDEX_DATE_AND_TIME],
                        columns[BookAdd.INDEX_ORDER_ID],
                        int.Parse(columns[BookAdd.QTD_OFFERS]));
                default:
                    return null;

            }
        }

        public MinibookMessage ParseError(string message)
        {
            //TODO generalizar tratamentos de erros
            string[] columns = message.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

            string stock = columns[columns.Length - 1];

            return new MinibookMessage(stock, CreateBookError(columns, stock));
        }
    }
}
