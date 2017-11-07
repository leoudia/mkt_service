using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Streaming.Common.Util.Contract;
using Streaming.Common.Util.List;
using Streaming.Common.Util.MessageFactory;
using Streaming.Crystal.Connection.Connection;
using Streaming.Crystal.Connection.Connection.Contract;
using Streaming.Crystal.Connection.Connection.Events;
using Streaming.Crystal.Connection.Manager;
using Streaming.Crystal.Connection.Model;
using Streaming.Crystal.Model.Base;
using Streaming.Crystal.Model.Model.CMP;
using Streaming.Crystal.Model.Model.Serialize;
using Streaming.Crystal.Notification.Contract;
using Streaming.Crystal.Notification.LocalThread;
using Streaming.Crystal.Notification.Parse.Book;
using Streaming.Crystal.Notification.Parse.Quote;
using Streaming.Crystal.Notification.PostCentral;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Difusion.Connection.Test
{
    public class Program
    {
        static ICrystalDifusionConnectionEvents connEvent;
        public static void Main(string[] args)
        {
            //TestBook();

            TestPostmanThread();
        }

        public static void TestBook()
        {
            var msg = "M:PETR3:A:0:A:13.08:100:15:11071759:0:5";
            BookParseProtocolCMP parse = new BookParseProtocolCMP();

            var bookCol11 = parse.Parse(msg);
            BookAdd bookAdd = (BookAdd)bookCol11.BookHeader;
            Console.WriteLine(bookAdd.BrokerId);

            msg = "M:PETR4:A:0:A:99.99:100:131:11041005";

            var bookCol9 = parse.Parse(msg);
            bookAdd = (BookAdd)bookCol9.BookHeader;
            Console.WriteLine(bookAdd.BrokerId);

            msg = "M:PETR3:D:3:5";

            var bookDel = parse.Parse(msg);
            var book = (BookDelete)bookDel.BookHeader;
            Console.WriteLine(book.DeleteType);
        }

        public static void TestParseQuote()
        {
            ILoggerFactory loggerFactory = new LoggerFactory()
            .AddConsole()
            .AddDebug();

            QuoteParseProtocolCMP parse = new QuoteParseProtocolCMP(loggerFactory.CreateLogger("parse"));
            string msg = "T:PETR3:181500:1:20170711:2:13.11:3:13.08:4:13.1:5:175751:6:0:7:800:8:6977:9:6839300:10:88956048:11:13.21:12:12.61:13:12.7:14:12.74:15:175900:16:175900:17:100:18:5300:19:100:20:5300:21:3.23:36:12.68:37:13.2:38:16.94:39:12.64:40:12.76:41:12.47:42:13.007:43:0:44:1:45:1:46:100:47:PETROBRAS: 48:ON: 49:1:50:00000000000000:51:00000000000000:52:0:53:0:54:20170711:56:V: 57:206400:58:000000:59:000000:60:15:61:21:62:3:63:45:64:00000000:65:0:66:7442454142:67:101:72:0:82:0:83:0:84:0:85:0:86:0.41:87:20170710:88:F: 89:12.672:96:RT: 100:7442454142:101:0:102:0:103:0:104:0:105:10021405:106:+:107:14.02:108:11.46:109:4:110:2:111:100:112:0.01:113:100:114:74424541:115:0:116:BRL: 117:CS: 118:1003:119:0:120:000000:121:0:122:0:123:1:124:0:125:00000000000000:126:10:127:0:128:0:129:00000000:130:0!";
            MessageBase q = parse.Parse(msg);

            Console.WriteLine(q.Type);
            Console.WriteLine(q.Id);
            Console.WriteLine(((QuoteMessage)q).Fields.Count);
        }

        public static void TestListCircular()
        {
            ListCircular<string> list = new ListCircular<string>();

            for(var i = 0; i <= 10; i++)
            {
                list.Add(Convert.ToString(i));
            }

            for (var i = 0; i <= 21; i++)
            {
                Console.WriteLine(list.Get());
            }
        }

        public static void TestPostmanThread()
        {

            ILoggerFactory loggerFactory = new LoggerFactory()
            .AddConsole()
            .AddDebug();

            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json");

            IConfigurationRoot config = builder.Build();

            ILogger<PostmanThread> logger = loggerFactory.CreateLogger<PostmanThread>();

            PostmanThread postmanThread = new PostmanThread(logger);
            var stock = "PETR4";

            postmanThread.AddChannelHandle(stock, Streaming.Crystal.Model.Type.MarketMessageType.MINI_BOOK,
                new FakeUserChannelNotification(delegate (MessageBase msg)
                {
                    BookMessageSerialize book = msg as BookMessageSerialize;
                    //logger.LogInformation($"book.id = {book.Id}");
                    //logger.LogInformation($"buy.count = {book.Buy.Count}");
                    //logger.LogInformation($"sell.count = {book.Sell.Count}");
                    
                    for(int i = 0; i < 5; i++)
                    {
                        var buy = book.Buy[i];
                        var sell = book.Sell[i];
                        logger.LogInformation($"buy price: {buy.Price} - qtd: {buy.Qtd} - date: {buy.DateAndTime}::::::" +
                            $"sell price: {sell.Price} - qtd: {sell.Qtd} - date: {sell.DateAndTime}" );
                    }

                    return true;
                }));

            IConnectionManager manager = new ConnectionManager(loggerFactory, config, new DirectNoficationMessage(postmanThread));

            ICrystalConnection conn = manager.NextConnection(); //CreateCrystalConnection(loggerFactory, postmanThread);
            
            conn.Connected += delegate (bool connected, EventArgs e)
            {
                if (connected)
                {
                    conn.Write(MessageFactoryCMP.SubscribeMiniBook(stock));
                }
            };

            if (conn.IsConnected())
            {
                conn.Write(MessageFactoryCMP.SubscribeMiniBook(stock));
            }

        }

        public static ICrystalDifusionConnectionBase CreateCrystalConnection(ILoggerFactory loggerFactory, PostmanThread postmanThread)
        {
            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

            IConfigurationRoot config = builder.Build();

            string user = config.GetSection("CrystalUserName").Value;
            string pass = config.GetSection("CrystalPassword").Value;

            IEnumerable<string> hosts = config.GetSection("CrystalHosts")
                .GetChildren()
                .Select(x => x.Value)
                .ToArray();

            string host = hosts.FirstOrDefault();
            int port = config.GetSection("CrystalPort").Value != null ? Convert.ToInt16(config.GetSection("CrystalPort").Value) : 81;

            DifusionCredential credential = new DifusionCredential(user, pass, "");

            ILogger<ICrystalDifusionConnectionBase> logger = loggerFactory.CreateLogger<ICrystalDifusionConnectionBase>();
            ILogger<CrystalDifusionConnectionEvents> loggerConn = loggerFactory.CreateLogger<CrystalDifusionConnectionEvents>();

            ListCircular<string> listCircular = new ListCircular<string>();
            listCircular.Add(host);

            ICrystalDifusionConnectionBase conn = new CrystalDifusionConnectionBase(new Streaming.Crystal.Connection.Manager.CrystalAddress(listCircular, port), logger);
            connEvent = new CrystalDifusionConnectionEvents(conn, new DirectNoficationMessage(postmanThread), loggerConn, "1", credential, new CrystalConnectionStatistics());

            return conn;
        }

        public static void TestCrystalConnection()
        {
            ILoggerFactory loggerFactory = new LoggerFactory()
            .AddConsole()
            .AddDebug();

            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

            IConfigurationRoot config = builder.Build();

            string user = config.GetSection("CrystalUserName").Value;
            string pass = config.GetSection("CrystalPassword").Value;

            IEnumerable<string> hosts = config.GetSection("CrystalHosts")
                .GetChildren()
                .Select(x => x.Value)
                .ToArray();

            string host = hosts.FirstOrDefault();
            int port = config.GetSection("CrystalPort").Value != null ? Convert.ToInt16(config.GetSection("CrystalPort").Value) : 81;

            DifusionCredential credential = new DifusionCredential(user, pass, "");

            ILogger<ICrystalDifusionConnectionBase> logger = loggerFactory.CreateLogger<ICrystalDifusionConnectionBase>();
            ILogger<CrystalDifusionConnectionEvents> loggerConn = loggerFactory.CreateLogger<CrystalDifusionConnectionEvents>();

            ListCircular<string> listCircular = new ListCircular<string>();
            listCircular.Add(host);

            ICrystalDifusionConnectionBase conn = new CrystalDifusionConnectionBase(new Streaming.Crystal.Connection.Manager.CrystalAddress(listCircular, port), logger);
            connEvent = new CrystalDifusionConnectionEvents(conn, new CrystalConnectionHandleImpl(), loggerConn, "1", credential, new CrystalConnectionStatistics());

            connEvent.Connected += ConnectedEvent;

            connEvent.Connect();
        }

        public static void ConnectedEvent(bool connected, EventArgs e)
        {
            Console.WriteLine("ConnectedEvent");
            connEvent.Write("sqt petr4");
        }

        public static void DisconnectEvent(bool connected, EventArgs e)
        {
            Console.WriteLine("DisconnectEvent");
        }

        class CrystalConnectionHandleImpl : CrystalConnectionMessageHandle
        {
            List<string> messages = new List<string>();
            public void Arrived(string message)
            {   
                messages.Add(message);
                Console.WriteLine("msg qtd: " + messages.Count);
                Console.WriteLine(message);
            }
        }

        class FakeUserChannelNotification : IUserChannelNotification
        {
            private Func<MessageBase, bool> functionValidate;

            public FakeUserChannelNotification(Func<MessageBase, bool> functionValidate)
            {
                this.functionValidate = functionValidate;
            }

            public void Arrived(MessageBase message)
            {
                functionValidate(message);
            }

            public bool Equals(IUserChannelNotification other)
            {
                return UserId().Equals(other.UserId());
            }

            public bool Equals(IUserChannelNotification x, IUserChannelNotification y)
            {
                return x.UserId().Equals(y.UserId());
            }

            public int GetHashCode(IUserChannelNotification obj)
            {
                return UserId().GetHashCode();
            }

            public string UserId()
            {
                return "user_fake_01";
            }
        }
    }
}
