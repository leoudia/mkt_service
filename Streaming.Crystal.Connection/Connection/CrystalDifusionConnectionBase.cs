using Streaming.Crystal.Connection.Connection.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Streaming.Crystal.Connection.Manager;

namespace Streaming.Crystal.Connection.Connection
{
    public class CrystalDifusionConnectionBase : ICrystalDifusionConnectionBase
    {

        #region "attributes"

        private TcpClient tcpClient;
        private StreamWriter streamWriter;
        private StreamReader streamReader;
        private ILogger<ICrystalDifusionConnectionBase> logger;
        private CrystalAddress crystalAddress;

        private const int BufferSize = 52428800;

        #endregion
        
        #region "events"

        #endregion

        public CrystalDifusionConnectionBase(CrystalAddress crystalAddress, ILogger<ICrystalDifusionConnectionBase> logger)
        {
            if (crystalAddress == null || string.IsNullOrEmpty(crystalAddress.GetCurrentHost()))
                throw new Exception("invalid host");

            if(crystalAddress.GetPort() <= 0)
                throw new Exception("invalid port");

            this.crystalAddress = crystalAddress;
            this.logger = logger;
        }

        #region "methods public"

        public async Task<bool> Open()
        {
            tcpClient = new TcpClient();
            tcpClient.ReceiveBufferSize = BufferSize;
            tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);

            await tcpClient.ConnectAsync(crystalAddress.GetHost(), crystalAddress.GetPort());

            if (tcpClient.Connected)
            {
                NetworkStream networkStream = tcpClient.GetStream();
                streamWriter = new StreamWriter(networkStream);
                streamReader = new StreamReader(networkStream);
                
                return true;
            }

            return false;
        }

        public string Read()
        {
            if (IsReadStream())
            {
                return streamReader.ReadLine();
            }
            else
            {
                throw new IOException("Disconnected Crystal Difusion");
            }
        }

        private bool IsReadStream()
        {
            return streamReader != null && streamReader.BaseStream != null && tcpClient.Connected && streamWriter.BaseStream.CanRead;
        }

        private bool IsWriteStream()
        {
            return streamWriter != null && streamWriter.BaseStream != null && streamWriter.BaseStream.CanRead;
        }

        public void Stop()
        {
            if(tcpClient != null && tcpClient.Connected)
            {
                try
                {
                    tcpClient.Client.Shutdown(SocketShutdown.Both);
                }catch(Exception e)
                {
                    logger.LogError("Stop", e);
                }
                finally
                {   
                    tcpClient.Dispose();
                }
            }
        }

        public void Write(string data)
        {
            if (IsWriteStream())
            {
                streamWriter.WriteLine(data);
                streamWriter.Flush();
            }
        }

        public bool IsConnected()
        {
            try
            {
                return IsReadStream();
            }
            catch (Exception e)
            {
                logger.LogError("CrystalDifusionConnectionBase::IsConnected:", e);
            }

            return false;
        }
        #endregion
    }
}
