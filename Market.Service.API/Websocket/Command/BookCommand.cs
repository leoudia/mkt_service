using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Market.Service.API.Websocket.Command
{
    public class BookCommand
    {
        public enum SubscribeType
        {
            SUBSCRIBE, UN_SUBSCRIBE
        }

        public SubscribeType CommandType { get; set; }

        public string Stock { get; set; }
    }
}
