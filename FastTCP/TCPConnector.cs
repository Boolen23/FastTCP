using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FastTCP
{
    public class TCPConnector
    {
        public TCPConnector(ConnectionType type, int port)
        {
            Conntype = type;
            Port = port;
            iPAddress = IPAddress.Any;
        }
        public TCPConnector(ConnectionType type, int port, string iP)
        {
            Conntype = type;
            iPAddress = IPAddress.Parse(iP);
            Port = port;
        }
        public async void Initialize()
        {
            if (Conntype == ConnectionType.Server)
            {
                tcpListener = new TcpListener(iPAddress, Port);
                await Task.Run(() => tcpListener.Start());
                await Task.Delay(500);
                tcpClient = await tcpListener.AcceptTcpClientAsync();
            }
            else
            {
                tcpClient = new TcpClient();
                await tcpClient.ConnectAsync(iPAddress, Port);
            }
        }
        private ConnectionType Conntype;
        private TcpListener tcpListener;
        private TcpClient tcpClient;
        private Task listenTask;
        private IPAddress iPAddress;
        private int Port;

    }
}
