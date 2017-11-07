using System;
using System.Collections.Generic;
using Streaming.Crystal.Model.Base;
using Microsoft.Extensions.Logging;
using Streaming.Crystal.Model.Model.CMP;

namespace Streaming.Crystal.Notification.Parse.Quote
{
    public class QuoteParseProtocolCMP : IParseMessageProtocolCMP<QuoteMessage>
    {
        //private int INDEX_TYPE = 0;
        private int INDEX_STOCK = 1;
        private int INDEX_TIME_LAST_MODIFY = 2;
        private int INDEX_START_FIELDS = 3;
        private char SEPARATOR_CARACTER = ':';
        private ILogger logger;
        private Dictionary<int, string> EmptyFileds { get; }

        public QuoteParseProtocolCMP(ILogger logger)
        {
            this.logger = logger;
            EmptyFileds = new Dictionary<int, string>();
        }

        public QuoteMessage Parse(string message)
        {
            if (string.IsNullOrEmpty(message) || message.IndexOf(SEPARATOR_CARACTER) == -1)
                return null;

            message = RemoveCaracterEndMessage(message);

            string[] fields = message.Split(new char[] { SEPARATOR_CARACTER }, StringSplitOptions.RemoveEmptyEntries);

            if (fields.Length < (INDEX_START_FIELDS + 1))
                return null;

            string stock = fields[INDEX_STOCK];
            Dictionary<int, string> fieldsMap = new Dictionary<int, string>();

            fieldsMap.Add(0, fields[INDEX_TIME_LAST_MODIFY]);

            for(int i = INDEX_START_FIELDS; i < fields.Length -1; i += 2)
            {
                try
                {
                    fieldsMap.Add(int.Parse(fields[i]), fields[i + 1]);
                }catch(Exception e)
                {
                    logger.LogError("Parse", e);
                }
            }

            return new QuoteMessage(stock, fieldsMap);
        }

        private string RemoveCaracterEndMessage(string message)
        {
            return message.Remove(message.Length - 1);
        }

        public QuoteMessage ParseError(string message)
        {
            string[] columns = message.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

            string stock = columns[columns.Length - 1];

            QuoteMessage q = new QuoteMessage(stock, EmptyFileds, true);

            return q;
        }
    }
}
